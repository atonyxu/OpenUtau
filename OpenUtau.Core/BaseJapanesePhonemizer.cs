using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using OpenUtau.Api;
using WanaKanaNet;
using OpenUtau.Core.G2p;
using OpenUtau.Core.Ustx;

namespace OpenUtau.Core {
    public abstract class BaseJapanesePhonemizer : Phonemizer {
        public static bool IsHanzi(string lyric) {
            return lyric.Length <= 2 && Regex.IsMatch(lyric, "[ぁ-んァ-ヴ]");
        }

        public static Note[] ChangeLyric(Note[] group, string lyric) {
            var oldNote = group[0];
            group[0] = new Note {
                lyric = lyric,
                phoneticHint = oldNote.phoneticHint,
                tone = oldNote.tone,
                position = oldNote.position,
                duration = oldNote.duration,
                phonemeAttributes = oldNote.phonemeAttributes,
            };
            return group;
        }

        public static string[] Romanize(IEnumerable<string> lyrics) {
            var lyricsArray = lyrics.ToArray();
            var hanziLyrics = String.Join(" ", lyricsArray
                .Where(IsHanzi));
            var pinyinResult = WanaKana.ToRomaji(hanziLyrics).ToLower().Split();
            var pinyinIndex = 0;
            for(int i=0; i < lyricsArray.Length; i++) {
                if (lyricsArray[i].Length <= 2 && Regex.IsMatch(lyricsArray[i], "[ぁ-んァ-ヴ]")) {
                    lyricsArray[i] = pinyinResult[pinyinIndex];
                    pinyinIndex++;
                }
            }
            return lyricsArray;
        }

        public static void RomanizeNotes(Note[][] groups) {
            var ResultLyrics = Romanize(groups.Select(group => group[0].lyric));
            Enumerable.Zip(groups, ResultLyrics, ChangeLyric).Last();
        }
        
        public override void SetUp(Note[][] groups, UProject project, UTrack track) {
            RomanizeNotes(groups);
        }
    }
}
