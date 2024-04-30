using GithubOctodexDownloader.Helpers;
using HtmlAgilityPack;
using System.IO;
using System.Windows;

namespace GithubOctodexDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string baseUrl = "https://octodex.github.com";
        string downloadPath = "Download/";
        bool isDownloading = false;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await DownLoad();
        }

        private async Task DownLoad()
        {
            if (isDownloading)
                return;

            isDownloading = true;
            await Task.Run(async () =>
            {
                var sourceText = await HttpHelper.GetAsync<string>(baseUrl);
                var doc = new HtmlDocument();
                doc.LoadHtml(sourceText);

                Directory.CreateDirectory("Download");

                //获取主页的所有图片链接
                var list = doc.DocumentNode.Descendants("div").Where(x => x.Attributes["class"]?.Value == "col-sm-6 col-lg-4 col-xl-3 mb-6 post").ToList();

                progress?.Dispatcher.InvokeAsync(() =>
                {
                    progress.Maximum = list.Count;
                });

                for (int i = 0; i <= list.Count - 1; i++)
                {
                    progress?.Dispatcher.InvokeAsync(() =>
                    {
                        progress.Value = i + 1;
                    });
                    var cla = list[i].Descendants("a").Where(x => x.Attributes["class"]?.Value == "link-gray-dark").ToList();
                    if (cla.Any())
                    {
                        //图片链接
                        var herf = baseUrl + cla[0].Attributes["href"].Value;

                        //进入子页面获取源码
                        sourceText = await HttpHelper.GetAsync<string>(herf);
                        var doc1 = new HtmlDocument();
                        doc1.LoadHtml(sourceText);

                        var imgNodes = doc1.DocumentNode.Descendants("img").Where(x => x.Attributes["class"]?.Value == "d-block width-fit height-auto mx-auto rounded-1").ToList();
                        foreach (var imgNode in imgNodes)
                        {
                            var img = baseUrl + imgNode.Attributes["src"].Value;

                            string fileName = downloadPath + Path.GetFileName(img);
                            if (!File.Exists(fileName))
                            {
                                txtShow?.Dispatcher.InvokeAsync(() =>
                                {
                                    txtShow.Text = $"{fileName}正在下载";
                                });
                                await HttpHelper.GetAsync<string>(img, fileName);
                            }
                            else
                            {
                                txtShow?.Dispatcher.InvokeAsync(() =>
                                {
                                    txtShow.Text = $"{fileName}已存在，跳过下载";
                                });
                            }
                        }
                    }
                }

                isDownloading = false;
            });
        }
    }
}