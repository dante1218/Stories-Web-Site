using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Serialization;  // must install Newtonsoft package from Nuget
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ClientGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string URL { get; set; }
        private string SetUserId { get; set; }
        private string StoryTitle { get; set; }
        private string Accessibility { get; set; }

        private string ReplaceSetStoryId { get; set; }
        private string ReplaceStoryTitle { get; set; }
        private string ReplaceAccessibility { get; set; }

        private string SetStoryId { get; set; }
        private string StoryImageURL { get; set; }
        private string StoryTextContent { get; set; }

        private string SetSlideId { get; set; }
        private string SlideImageURL { get; set; }
        private string SlideTextContent { get; set; }

        private string SetDeleteStoryId { get; set; }
        private string SetDeleteSlideId { get; set; }

        private string serverURL = "https://localhost:44305/api/File/";
        private string apiURL = "";
        private HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
        }

        //detect input change of Upload Slide view
        private void StoryTextChanged(object sender, RoutedEventArgs e)
        {
            SetStoryId = StoryIdBox.Text;
            StoryImageURL = StoryImageBox.Text;
            StoryTextContent = StoryTextBox.Text;
        }

        //detect input change of Replace Slide view
        private void SlideTextChanged(object sender, RoutedEventArgs e)
        {
            SetSlideId = SlideIdBox.Text;
            SlideImageURL = SlideImageBox.Text;
            SlideTextContent = SlideTextBox.Text;
        }

        //detect input change of Create Story view
        private void StoryUserTextChanged(object sender, RoutedEventArgs e)
        {
            SetUserId = StoryUserIdBox.Text;
            StoryTitle = StoryTitleBox.Text;
            Accessibility = StoryPrivateBox.Text;
        }

        //detect input change of Replace Story view
        private void ReplaceTextChanged(object sender, RoutedEventArgs e)
        {
            ReplaceSetStoryId = ReplaceStoryIdBox.Text;
            ReplaceStoryTitle = ReplaceTitleBox.Text;
            ReplaceAccessibility = ReplacePrivateBox.Text;
        }

        //detect input change of Delete Story view
        private void DeleteStoryTextChanged(object sender, RoutedEventArgs e)
        {
            SetDeleteStoryId = DeleteStoryIdBox.Text;
        }

        //detect input change of Delete Slide view
        private void DeleteSlideTextChanged(object sender, RoutedEventArgs e)
        {
            SetDeleteSlideId = DeleteSlideIdBox.Text;
        }

        //fetch image file for story
        private void StoryFindImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != null)
            {
                string filePath = openFileDialog.FileName;
                StoryImageBox.Text = filePath;
                //Console.Write(filePath);
            }
        }

        //fetch image file for slide
        private void SlideFindImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != null)
            {
                string filePath = openFileDialog.FileName;
                SlideImageBox.Text = filePath;
                //Console.Write(filePath);
            }
        }

        //check file 
        private bool CheckFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                return true;
            }
            return false;
        }

        //make upload slide request
        private void UploadRequest(object sender, RoutedEventArgs e)
        {
            if (CheckFile(StoryImageURL) == false) StoryImageURL = null;

            if (SetStoryId == null || SetStoryId == "") return;

            apiURL = serverURL + "uploadSlide";
            Task<HttpResponseMessage> result = SendFile(StoryImageURL, StoryTextContent, "story", SetStoryId);
        }

        //make replace slide request
        private void ReplaceRequest(object sender, RoutedEventArgs e)
        {
            if (CheckFile(SlideImageURL) == false) SlideImageURL = null;

            if (SetSlideId == null || SetSlideId == "") return;

            apiURL = serverURL + "replaceSlide";
            Task<HttpResponseMessage> result = SendFile(SlideImageURL, SlideTextContent, "slide", SetSlideId);
        }

        //make create story request
        private void CreateStoryRequest(object sender, RoutedEventArgs e)
        {
            if (SetUserId == null && SetUserId == "") return;

            apiURL = serverURL + "uploadStory";

            Task<HttpResponseMessage> result = SendStoryRequest(SetUserId, StoryTitle, Accessibility);
        }

        //make replace story request
        private void ReplaceStoryRequest(object sender, RoutedEventArgs e)
        {
            if (ReplaceSetStoryId == null && ReplaceSetStoryId == "") return;

            apiURL = serverURL + "replaceStory";

            Task<HttpResponseMessage> result = SendStoryRequest(ReplaceSetStoryId, ReplaceStoryTitle, ReplaceAccessibility);
        }

        //send story request
        private async Task<HttpResponseMessage> SendStoryRequest(string id, string title, string accessibility)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            multiContent.Add(new StringContent(id), "id");
            multiContent.Add(new StringContent(title), "title");
            multiContent.Add(new StringContent(accessibility), "private");

            return await client.PostAsync(apiURL, multiContent);
        }

        //send slide request
        private async Task<HttpResponseMessage> SendFile(string image, string text, string key, string keyValue)
        {
            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            if (image != null && image != "")
            {
                byte[] data = System.IO.File.ReadAllBytes(image);
                ByteArrayContent bytes = new ByteArrayContent(data);
                string fileName = System.IO.Path.GetFileName(image);
                multiContent.Add(bytes, "files", fileName);
            }

            if (text != null)
            {
                multiContent.Add(new StringContent(text), "textContent");
            }
            multiContent.Add(new StringContent(keyValue), key);


            return await client.PostAsync(apiURL, multiContent);
        }

        //make delete story request
        private void DeleteStoryRequest(object sender, RoutedEventArgs e)
        {
            if (SetDeleteStoryId == null && SetDeleteStoryId == "") return;

            apiURL = serverURL + "story=";
            Task<HttpResponseMessage> result = SendDelete(SetDeleteStoryId);
        }

        //make delete slide request
        private void DeleteSlideRequest(object sender, RoutedEventArgs e)
        {
            if (SetDeleteSlideId == null && SetDeleteSlideId == "") return;

            apiURL = serverURL + "slide=";
            Task<HttpResponseMessage> slide = SendDelete(SetDeleteSlideId);
        }

        //send delete request
        private async Task<HttpResponseMessage> SendDelete(string id)
        {
            return await client.DeleteAsync(apiURL + id);
        }
    }
}
