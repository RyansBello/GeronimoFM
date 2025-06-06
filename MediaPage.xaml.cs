using Geronimo.LinkStream;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Geronimo;
public partial class MediaPage : INotifyPropertyChanged
{
    private bool isPlay = false;
    private string play_Status = "▶️ Play";
    private Color buttonColor = Colors.YellowGreen;
    private string audioSource;

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

    public MediaPage()
    {
        InitializeComponent();
        AudioSource = LinkStream.LinkStream.GetLinkStream(); // gunakan properti AudioSource
        BindingContext = this;
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
}
