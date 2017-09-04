using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestApiApp.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace RestApiApp
{
    public class Post : INotifyPropertyChanged
    {
        private string _title;
        private string _body;

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("body")]
        public string Body
        {
            get { return _body; }
            set
            {
                _body = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _client = new HttpClient();
        private ObservableCollection<Post> _observablePosts;
        private const string Url = "http://jsonplaceholder.typicode.com/posts";

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            var content = await _client.GetStringAsync(Url);
            var posts = JsonConvert.DeserializeObject<List<Post>>(content);

            _observablePosts = new ObservableCollection<Post>(posts);

            MyListView.ItemsSource = _observablePosts;

            base.OnAppearing();
        }

        private async void OnAdd(object sender, EventArgs e)
        {
            var post = new Post { Title = "Hello World", Body = "Welcome to Xamarin Forms" };
            var content = JsonConvert.SerializeObject(post);
            _observablePosts.Insert(0, post);
            await _client.PostAsync(Url, new StringContent(content));
        }

        private async void OnUpdate(object sender, EventArgs e)
        {
            var post = _observablePosts[0];
            post.Title += " [latest version]";
            var content = JsonConvert.SerializeObject(post);
            await _client.PutAsync(Url + "/" + post.Id, new StringContent(content));
        }

        private void OnDelete(object sender, EventArgs e)
        {
            var post = _observablePosts[0];
            _observablePosts.Remove(post);
            _client.DeleteAsync(Url + "/" + post.Id);
        }
    }
}