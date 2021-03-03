module Parser

open FParsec


type Token = {
    Duration: int
    isExtended: bool
    isSemiTone: bool
    Note: char
    Octave: int
} // TODO 1 create types for language

let durationParser: Parser<int32, unit> =
    pint32

let extendedParser: Parser<bool, unit> =
    stringReturn "." true
    <|> stringReturn "" false


let semiToneParser: Parser<bool, unit> =
    stringReturn "#" true
    <|> stringReturn "" false

let noteParser: Parser<char, unit> =
    anyOf "abcdefg"


let octaveParser: Parser<int32, unit> =
    pint32

let tokenPibe = pipe5 durationParser extendedParser semiToneParser noteParser octaveParser (fun d e s n o -> {
    Duration = d
    isExtended = e 
    isSemiTone = s
    Note = n
    Octave = o
})

let pScore: Parser<Token list, unit> = sepBy tokenPibe (pstring " ") // TODO 2 builder parser

let parse (input: string): Choice<string, Token list> =
    match run pScore input with
    | Failure(errorMsg,_,_)-> Choice1Of2(errorMsg)
    | Success(result,_,_) -> Choice2Of2(result)

// Helper function to test parsers
let test (p: Parser<'a, unit>) (str: string): unit =
    match run p str with
    | Success(result, _, _) ->  printfn "Success: %A" result
    | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg


// TODO 3 calculate duration from token.
// bpm = 120 (bpm = beats per minute)
// 'Duration in seconds' * 1000 * 'seconds per beat' (if extended *1.5)
// Half note: 2 seconds
// Quarter note: 1 second
// Eight note: 1/2 second
// Sixteenth note 1/4 second
// thirty-second note: 1/8
let durationFromToken (token: Token): float = 
    let noteDuration = match token.Duration with 
        | 1 -> 4.0
        | 2 -> 2.0
        | 4 -> 1.0
        | 8 -> 0.5
        | 16 -> 0.25
        | 32 -> 0.125
    let duration = match token.isExtended with
        | true ->  noteDuration * 1000.0 * 0.5 * 1.5
        | false -> noteDuration * 1000.0 * 0.5
    duration

let semiTones = [
    (('c', false),1);
    (('c', true),2);
    (('d',false),3)
    (('d',true),4)
    (('e',false),5)
    (('e',true),6)
    (('f',false),6)
    (('f',true),7)
    (('g',false),8)
    (('g',true),9)
    (('a',false),10)
    (('a',true),11)
    (('b',false),12)
    (('b',true),13)] |> Map.ofSeq

// TODO 4 calculate overall index of octave
// note index + (#octave-1 * 12)
let overallIndex (note, octave) = 
    (octave-1)*12 + semiTones.[note]

// TODO 5 calculate semitones between to notes*octave
// [A; A#; B; C; C#; D; D#; E; F; F#; G; G#]
// overallIndex upper - overallIndex lower
let semitonesBetween lower upper = 
    (float)(overallIndex upper - overallIndex lower)

// TODO 6
// For a tone frequency formula can be found here: http://www.phy.mtu.edu/~suits/NoteFreqCalcs.html
// 220 * 2^(1/12) * semitonesBetween (A1, Token.pitch) 
let frequency (token: Token): float = 
    220. * (2.**(1./12.)) ** semitonesBetween (('a',false),1) ((token.Note,token.isSemiTone),token.Octave)
