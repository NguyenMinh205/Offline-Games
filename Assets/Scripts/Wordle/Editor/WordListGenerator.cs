using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace NguyenQuangMinh.Wordle
{
    public class WordListGenerator
    {
        [MenuItem("Tools/Generate Word Lists (txt → ScriptableObject)")]
        static void Generate()
        {
            string[] solutionsRaw = Resources.Load<TextAsset>("Data/Wordle/wordle_solutions").text
                .Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().ToUpper())
                .ToArray();

            string[] validsRaw = Resources.Load<TextAsset>("Data/Wordle/wordle_validwords").text
                .Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().ToUpper())
                .ToArray();

            WordDatabase so = ScriptableObject.CreateInstance<WordDatabase>();
            so.solutions = new List<string>(solutionsRaw);
            so.validWords = new List<string>(validsRaw);

            string path = "Assets/Resources/Data/Wordle/WordDatabase.asset";
            AssetDatabase.CreateAsset(so, path);
            AssetDatabase.SaveAssets();

            Debug.Log($"Generated: {so.solutions.Count} solutions & {so.validWords.Count} valid words");
        }
    }
}