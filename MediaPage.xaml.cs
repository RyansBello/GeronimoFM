using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using Geronimo.LinkStream;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Geronimo;
public partial class MediaPage : INotifyPropertyChanged
{
    private bool isPlay = false;
    private string play_Status = "▶️ Play";
    private Color buttonColor = Colors.YellowGreen;
    private string audioSource;
    private CancellationTokenSource? _cts;
    private PeriodicTimer? _timer;
    string dataArtist = "Now Playing";

    public class Artist
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class Album
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class MusicItem
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("album")]
        public Album Album { get; set; }

        [JsonPropertyName("artists")]
        public List<Artist> Artists { get; set; }
    }

    public class Metadata
    {
        [JsonPropertyName("music")]
        public List<MusicItem> Music { get; set; }
    }

    public class DataItem
    {
        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("data")]
        public List<DataItem> Data { get; set; }
    }

    public int ConVolume
    {
        get 
        {
            if (lblVolume != null)
            {
                return (int)(audioMediaElement.Volume * 100);
            }
            else return 100;
        }

        set
        {
            if (audioMediaElement != null && audioMediaElement.Volume != value)
            {
                OnPropertyChanged(nameof(ConVolume));
            }
        }
    }

    public double Volume
    {
        get
        {
            if (audioSource != null)
            {               
                return audioMediaElement.Volume;
            }
            else return 1;
        }
        set
        {
            bool setFlag = false;            
            if (audioMediaElement != null && audioMediaElement.Volume != value)
            {
                audioMediaElement.Volume = value;
                setFlag = true;
            }
            if (setFlag)
            { 
                OnPropertyChanged(nameof(Volume));
            }
        }
    }

    public string Play_Status
    {
        get => play_Status;
        set
        {
            if (play_Status != value)
            {
                play_Status = value;
                OnPropertyChanged(nameof(Play_Status));
            }
        }
    }

    public Color ButtonColor
    {
        get => buttonColor;
        set
        {
            if (buttonColor != value)
            {
                buttonColor = value;
                OnPropertyChanged(nameof(ButtonColor));
            }
        }
    }

    public string AudioSource
    {
        get => audioSource;
        set
        {
            if (audioSource != value)
            {
                audioSource = value;
                OnPropertyChanged(nameof(AudioSource));
            }
        }
    }

    private string _metaArtist;
    private string _metaTitle;

    public string MetaArtist
    {
        get => _metaArtist;
        set
        {
            _metaArtist = value;
            OnPropertyChanged(nameof(MetaArtist));
        }
    }

    public string MetaTitle
    {
        get => _metaTitle;
        set
        {
            _metaTitle = value;
            OnPropertyChanged(nameof(MetaTitle));
        }
    }

    public void UpdateMetadata(string artist, string title)
    {
        MetaArtist = artist;
        MetaTitle = title;
        Console.WriteLine($"[DEBUG] Metadata diperbarui: Artist={MetaArtist}, Title={MetaTitle}");
    }

    public MediaPage()
    {
        InitializeComponent();
        CanBeDismissedByTappingOutsideOfPopup = false;
        AudioSource = LinkStream.LinkStream.GetLinkStream(); // gunakan properti AudioSource
        BindingContext = this;
        
        this.Opened += PlayerPopup_Opened;
        this.Closed += PlayerPopup_Closed;
    }

    private void StartPlay()
    {
        isPlay = true;
        Play_Status = "⏹️ Stop";
        ButtonColor = Colors.Red;
        audioMediaElement.Play();

    }

    private void StopPlay()
    {        
        audioMediaElement.Stop();
        Close();
    }

    private void Play_Clicked(object sender, EventArgs e)
    {
        if (isPlay)
            StopPlay();
        else
            StartPlay();
    }

    private void VerticalStackLayout_Loaded(object sender, EventArgs e)
    {
        StartPlay();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        // Konversi dari nilai slider (0.0 - 1.0) ke posisi X (misalnya antara 0 dan 210)
        double maxX = 210; // Total width container (250) - width label (40)
        double newX = e.NewValue * maxX;

        lblVolume.TranslationX = newX;

        // Update volume audio
        if (audioMediaElement != null)
        {
            audioMediaElement.Volume = e.NewValue;
        }

        indVolume.Text = $"Volume : 🔊 {(int)(e.NewValue * 100)} %";
    }

    private void PlayerPopup_Opened(object? sender, EventArgs e)
    {
        _cts = new CancellationTokenSource();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        _ = RefreshLoopAsync(_cts.Token);
    }

    private void PlayerPopup_Closed(object? sender, PopupClosedEventArgs e)
    {
        _cts?.Cancel();
        _timer?.Dispose();
    }

    private async Task RefreshLoopAsync(CancellationToken token)
    {
        try
        {
            while (await _timer!.WaitForNextTickAsync(token))
            {
                if (audioMediaElement.CurrentState == MediaElementState.Playing)
                {
                    var info = await GetSongInfoAsync();
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        lblNowPlaying.Text = info;
                    });
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal saat popup ditutup
        }
    }

    public async Task<string> GetSongInfoAsync()
    {
        string dtresult = "Fetch data...";

        try
        {
            var url = "https://api-v2.acrcloud.com/api/bm-cs-projects/16166/streams/s-UShnaIBQ/results?type=last";
            var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiI3IiwianRpIjoiNjE1OTg5ODA0MjhhZWMxZWM2YTMxZTU4Nzg1MDMyMzA4NmRhY2ZiYTA0ZjQ3NjhhNWVmMTY0ZWU2ZDllNjRjMjhjZGJmMTZjMjNiYmU2MWEiLCJpYXQiOjE3NDkyNTkzODAuMzIzMjgzLCJuYmYiOjE3NDkyNTkzODAuMzIzMjg1LCJleHAiOjIwNjQ3OTIxNzguMDcyODEsInN1YiI6IjIzNDU5NSIsInNjb3BlcyI6WyJibS1wcm9qZWN0cyIsImJtLWNzLXByb2plY3RzIiwid3JpdGUtYm0tY3MtcHJvamVjdHMiLCJyZWFkLWJtLWNzLXByb2plY3RzIiwiYm0tYmQtcHJvamVjdHMiLCJ3cml0ZS1ibS1iZC1wcm9qZWN0cyIsInJlYWQtYm0tYmQtcHJvamVjdHMiXX0.iQSnDieGik7fV80WwNmivyu9upayKvUmR-dp9wJD_zaY7ZmPCGjqC8J4q3l-gQZuV6kswVDOpzQQoyM-76Fug4hTMU1fL4k5OdCgHkKWyKVOB9XhphghdHm2xUzuK5WULOmmbdjkSJkuYw6Pqr2h9HRBCheKEm_0FgLVImQiXej--wBL-rWIrWiV2cUaewlVvaFNcvIY_HLr81AIEw9BpOpeGbvMa7U2yJi6gZS-DveDbQOF3NFs8cQXmXi0Bnv-nK4Fc86tI0To8dRX4u-MwPdPVNG3UbxB65FmbQwQIKmR5xR6o7u-uiUjjBI4cPt3lYAUJiPqAItgxVJtrpsjf1t8AYmSurMz5HrpDmB4PRfjaLbLjSkggHNLkWo1FEWtO1Tb-dJIGToZAcVQsTySObbQNpppObuhw6zu7tzXtbVvDtwoXAFVOOAwfUSMN6slCc5EbHsc-6r1CZwmv4WaOAeBEY40d3geMca0RO0o0718jNjpnAidZua7QDQd7bF-8Ns5o-Q-HkxiN7dpxlKZQgxNQ7KN77jiwubu4sBsYyJi1sm97DPMTKhpUBvBLMlSST3Zji8Aysi95Y1ZNOWKtMpvyiQ0qiWBtbgU7WhgXREHStHMQL-Kh3NWakSBaOTkGkqRSd6xSQTfXAgr6tVPEw-9R9f8aYtmpXmd1RgS1I4"; // token dipersingkat

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<Root>(json, options);
            var music = result?.Data?.FirstOrDefault()?.Metadata?.Music?.FirstOrDefault();

            if (music != null)
            {
                var artistName = music.Artists?.FirstOrDefault()?.Name;
                var title = music.Title;
                var albumName = music.Album?.Name;

                dtresult = $"{artistName} - {title}";

                var viewModel = BindingContext as MediaPage;
                string newArtist = String.IsNullOrWhiteSpace(artistName) ? "Geronimo FM" : artistName;
                string newTitle = String.IsNullOrWhiteSpace(title) ? "Now Playing" : title;

                viewModel?.UpdateMetadata(newArtist, newTitle);
            }
        }
        catch (Exception ex)
        {
            dtresult = $"Error: {ex.Message}";
        }

        return dtresult;
    }
}
