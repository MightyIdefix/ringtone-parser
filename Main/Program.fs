
open System
open Parser
open WavePacker

// TODO 8 take input from file/cmd and save it to a file
[<EntryPoint>]
let main argv =
    let pinkPanther = "8#g1 2a1 8b1 2c2 8#g1 8a1 8b1 8c2 8f2 8e2 8a1 8c2 8e2 2#d2 16d2 16c2 16a1 8g1 1a1 8#g1 2a1 8b1 2c2 8#g1 8a1 8b1 8c2 8f2 8e2 8c2 8e2 8a2 1#g2 8#g1 2a1 8b1 2c2 16#g1 8a1 8b1 8c2 8f2 8e2 8a1 8c2 8e2 2#d2 8d2 16c2 16a1"
    match Assembler.assembleToPackedStream pinkPanther with
        | Choice2Of2 ms -> write "mySong.wav" ms
        | Choice1Of2 err -> failwith err



    (*let score = "32.#d3 16-"
    let token = parse score

    let testToken = {
        Duration  = 2
        isExtended = false
        isSemiTone = false
        Note = 'c'
        Octave = 2
    }


    let duration = durationFromToken testToken
    printf "Duration: %A" duration
    let overallindextest = overallIndex ((testToken.Note, testToken.isSemiTone), testToken.Octave)
    printf "overallindex: %i" overallindextest
    let testFrekvens = frequency testToken
    printf "frekvens: %f" testFrekvens
    *)


    0