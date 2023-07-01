using System.Numerics;
using AutoARPG_WebAsm.Scenes;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Dithering;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace AutoARPG_Web.Data;


public class GameRenderer
{
    public IScene ActiveScene;
    public GameRenderer(GameState state, IScene scene)
    {
        _state = state;
        ActiveScene = scene;
        pixels = Enumerable.Range(0, GameState.width * GameState.height)
            .Select((i, i1) => GetPixel(new Rgb24(0, 0, 0), i, 0)).ToArray();
    }

    private readonly GameState _state;
    private readonly IJSRuntime _jsRuntime;
    private Rgb24[] pixels;

    public async Task GenImage()
    {
        using Image<Rgb24> gif = Image.LoadPixelData<Rgb24>(pixels.Select((pix, i) => GetPixel(pix, i, 0)).ToArray(),
            GameState.width, GameState.height);

        var gifMetaData = gif.Metadata.GetGifMetadata();
        gifMetaData.RepeatCount = ushort.MaxValue;
        GifFrameMetadata metadata = gif.Frames.RootFrame.Metadata.GetGifMetadata();
        metadata.FrameDelay = 100;

        for (int f = 1; f < 15; f++)
        {
            gif.Frames.AddFrame(pixels.Select((pix, i) => GetPixel(pix, i, f)).ToArray());
            var fMetadata = gif.Frames[^1].Metadata.GetGifMetadata();
            fMetadata.FrameDelay = 7;
        }


        var str = new MemoryStream();
        var encoder = new GifEncoder()
        {
            Quantizer = new WebSafePaletteQuantizer()
            {
                Options = {Dither = OrderedDither.Ordered3x3}
            }
        };
        await encoder.EncodeAsync(gif, str);
        str.TryGetBuffer(out ArraySegment<byte> buffer);
        ImageUrl =
            $"data:{GifFormat.Instance.DefaultMimeType};base64,{Convert.ToBase64String(buffer.Array ?? Array.Empty<byte>(), 0, (int) str.Length)}";
    }

    private Vector2 MakePos(int index)
    {
        var mod = index % GameState.width;
        var pX = mod / (float) GameState.width;

        var mulY = index - mod;
        var indY = mulY / GameState.width;
        var pY = indY / (float) GameState.height;
        return new Vector2(pX, pY);
    }

    private Rgb24 GetPixel(Rgb24 pix, int i, int frame)
    {
        var position = MakePos(i);

        return ActiveScene.Render(pix, position, frame);
    }

    public string ImageUrl;
}

public class GameStateService
{
    public Task<GameRenderer> GetStateAsync<T>(GameState? state) where T : IScene, new() =>
        Task.FromResult(new GameRenderer(state ?? new GameState(), new T()));
}