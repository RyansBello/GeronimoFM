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

    public MediaPage()
    {
        InitializeComponent();
        CanBeDismissedByTappingOutsideOfPopup = false;
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
}
