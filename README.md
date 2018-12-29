# youtube-dl Handler

A tool that allows you to call [youtube-dl](https://rg3.github.io/youtube-dl/) from a protocol URI (`ytdl:`).  
A small registry change links this protocol to the application, allowing you to download `m4a` audio files directly from a YouTube page.

| ![](https://i.imgur.com/aYzPHib.png) |
|---|

I found that existing solutions usually relied on online services that didn't do a good job at preserving audio quality. This tool uses the reputable [youtube-dl](https://rg3.github.io/youtube-dl/) CLI to download high quality files.

## Prerequisites

* [.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653) or higher.
* [Tampermonkey](https://tampermonkey.net/)

## Installation

Download the release from the [Release page](https://github.com/Silverfeelin/youtube-dl-handler/releases).

**Handler**  
* Unpack `YoutubeDLHandler.exe` somewhere.
  * For example, `C:\Program Files\YoutubeDLHandler\YoutubeDLHandler.exe`.
* Run `YoutubeDLHandler.exe` once to download `youtube-dl` and configure the download location.

**FFmpeg**  
It is highly recommended to [download FFmpeg](https://ffmpeg.zeranoe.com/builds/). This allows youtube-dl to convert downloaded `webm` files to audio files.

* Unpack both `bin/ffmpeg.exe` and `bin/ffprobe.exe` to `%AppData%/youtube-dl/`.

**Userscript**

* Install https://greasyfork.org/en/scripts/376070-youtube-m4a-handler

**Regedit**  
* Unpack `ytdl.reg` somewhere and open it in a text editor.
* Change the path to your `YoutubeDLHandler.exe` location. Save the file.
* Run `ytdl.reg` once to link the protocol to the application.

## Usage

Go to a YouTube video and press the <kbd>Download M4A</kbd> button. That's it! For other formats you can modify the Tampermonkey script.

---

## Protocol

**`ytdl:OPTIONS`**  
Example: `ytdl:-x --audio-format m4a --audio-quality 0 https://www.youtube.com/watch?v=q1ULJ92aldE`

`OPTIONS` can be replaced with any [youtube-dl options](https://github.com/rg3/youtube-dl/blob/master/README.md#options). Check out the Tampermonkey script for a working example.

Default options appended by the handler are `--newline --no-playlist -o {destination}`. These options should not be (re)used in the protocol.
