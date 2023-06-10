using System.Numerics;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace AutoARPG_Web.Data;

[Serializable]
public class GameState
{
    public int width = 160;
    public int height = 120;
    public int TagWidth => width * 5;
    public int TagHeight => height * 5;
}

public class GameRenderer
{
    public GameRenderer(GameState state, IJSRuntime jsRuntime)
    {
        _state = state;
        _jsRuntime = jsRuntime;
        pixels = Enumerable.Range(0, _state.width * _state.height)
            .Select((i, i1) => GetPixel(new Rgb24(0, 0, 0), i, 0)).ToArray();
    }

    private readonly GameState _state;
    private readonly IJSRuntime _jsRuntime;
    private Rgb24[] pixels;

    public async Task GenImage()
    {
        await _jsRuntime.InvokeVoidAsync("console.time", "firstFrame");
        using Image<Rgb24> gif = Image.LoadPixelData<Rgb24>(pixels.Select((pix, i) => GetPixel(pix, i, 0)).ToArray(),
            _state.width, _state.height);

        await _jsRuntime.InvokeVoidAsync("console.timeEnd", "firstFrame");
        await _jsRuntime.InvokeVoidAsync("console.time", "setMetadata");
        var gifMetaData = gif.Metadata.GetGifMetadata();
        gifMetaData.RepeatCount = ushort.MaxValue;
        GifFrameMetadata metadata = gif.Frames.RootFrame.Metadata.GetGifMetadata();
        metadata.FrameDelay = 200;

        await _jsRuntime.InvokeVoidAsync("console.timeEnd", "setMetadata");
        for (int f = 1; f < 15; f++)
        {
            await _jsRuntime.InvokeVoidAsync("console.time", $"frame {f}");
            gif.Frames.AddFrame(pixels.Select((pix, i) => GetPixel(pix, i, f)).ToArray());
            GifFrameMetadata fmetadata = gif.Frames[gif.Frames.Count-1].Metadata.GetGifMetadata();
            fmetadata.FrameDelay = 15;
            await _jsRuntime.InvokeVoidAsync("console.timeEnd", $"frame {f}");
        }

        await _jsRuntime.InvokeVoidAsync("console.time", "retrievePixels - 1");

        // var returned = gif.ToBase64String();
        var str = new MemoryStream();
        GifEncoder encoder = new GifEncoder()
        {
            Quantizer = new WebSafePaletteQuantizer()
        };
        await _jsRuntime.InvokeVoidAsync("console.timeEnd", "retrievePixels - 1");
        await _jsRuntime.InvokeVoidAsync("console.time", "retrievePixels - 2");
        await gif.SaveAsync(str, encoder);
        await _jsRuntime.InvokeVoidAsync("console.timeEnd", "retrievePixels - 2");
        await _jsRuntime.InvokeVoidAsync("console.time", "toBase64");
        str.TryGetBuffer(out ArraySegment<byte> buffer);
        ImageUrl = $"data:{GifFormat.Instance.DefaultMimeType};base64,{Convert.ToBase64String(buffer.Array ?? Array.Empty<byte>(), 0, (int) str.Length)}";
        await _jsRuntime.InvokeVoidAsync("console.timeEnd", "toBase64");
    }

    private Vector2 MakePos(int index)
    {
        var mod = index % _state.width;
        var pX = mod / (float) _state.width;

        var mulY = index - mod;
        var indY = mulY / _state.width;
        var pY = indY / (float) _state.height;
        return new Vector2(pX, pY);
    }

    private Rgb24 GetPixel(Rgb24 pix, int i, int frame)
    {
        var position = MakePos(i);
        pix.R = (byte) (position.X * 255);
        pix.G = (byte) (position.Y * 255);
        pix.B = (byte) (frame / 10f * 255f);
        return pix;
    }

    public string ImageUrl;
}

public class GameStateService
{
    public Task<GameRenderer> GetStateAsync(GameState? state, IJSRuntime jsRuntime) =>
        Task.FromResult(new GameRenderer(state ?? new GameState(), jsRuntime));
}