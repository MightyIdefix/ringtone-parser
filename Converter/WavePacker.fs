module WavePacker

open System.IO
open System.Text

// TODO 7 write data to stream
// Inspiration: https://www.codeproject.com/Questions/730926/Writting-Header-wav-in-wav-file  
// http://soundfile.sapp.org/doc/WaveFormat/
// subchuncksize 16
// audioformat: 1s
// num channels: 1s
// sample rate: 44100
// byte rate: sample rate *16/8
// block origin: 2s
// bits per sample: 16s

//Refences https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding.getbytes?view=net-5.0
    //References https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter.write?view=net-5.0
    //HINTS Encoding.ASCII.getbuty - GetBytes(String) When overridden in a derived class, encodes all the characters in the specified string into a sequence of bytes.
    // writer.write(44100) - Write(Char[]) Writes a character array to the stream.

let byteRateCal = 44100 * 16/8 //Ikke sikker på, men jeg følger formlen på siden http://soundfile.sapp.org/doc/WaveFormat/
let blockAlignCal = 2s //Jeg følger formlen på http://soundfile.sapp.org/doc/WaveFormat/ - giver det samme som Henrik skriver for block origin


let pack (d: int16[]) =
    let stream = new MemoryStream()
    let writer = new BinaryWriter(stream, Encoding.ASCII)
    let dataLength = Array.length d*2
    
    // RIFF chunk descriptor
    let chunkID = writer.Write(Encoding.ASCII.GetBytes("RIFF"))
    let chunkSize = 36 + dataLength in writer.Write(chunkSize)
    let format = writer.Write(Encoding.ASCII.GetBytes("WAVE"))

    //fmt sub-chunk
    let subChunk1ID = writer.Write(Encoding.ASCII.GetBytes("fmt ")) //mellemrummet skal være der
    let subchunck1size = writer.Write(16)
    let audioformat = writer.Write(1s)
    let numChannels = writer.Write(1s)
    let sampleRate = writer.Write(44100)
    let byteRate = writer.Write(byteRateCal) 
    let blockAlign = writer.Write(blockAlignCal) 
    let bitsPerSample = writer.Write(16s)
    
    
    // data
    let subChunk2ID = writer.Write(Encoding.ASCII.GetBytes("data"))
    let subchunck2size = writer.Write(dataLength-44)
    let data:byte[] = Array.zeroCreate dataLength
    System.Buffer.BlockCopy(d,0,data,0,data.Length)
    writer.Write(System.Buffer.ByteLength(data)*2)
    writer.Write(data) 
    stream

//Returner med denne funktion     
let write filename (ms: MemoryStream) =
    use fs = new FileStream(Path.Combine(__SOURCE_DIRECTORY__, filename), FileMode.Create) // use IDisposible
    ms.WriteTo(fs)
