using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class LSystem
{
    [SerializeField]
    public string sentence;
    public string originalSentence;

    [SerializeField]
    private List<Rule> rules = new List<Rule>();

    public int RuleCount
    {
        get { return rules.Count; }
    }

    public string GeneratedSentence
    {
        get { return sentence; }
    }
    public void SaveOriginalSentence()
    {
        if (string.IsNullOrEmpty(originalSentence))
        {
            originalSentence = sentence;
        }
    }
    public void RestoreToOriginalSentence()
    {
        sentence = originalSentence;
    }

    public void Generate(int generations)
    {
        var dict = rules.ToDictionary(x => x.ruleCharacter, x => x.ruleReplacement);

        StringBuilder iteration = new StringBuilder();

        for (int i = 0; i < generations; i++)
        {
            foreach (char c in sentence)
            {
                iteration.Append(dict.ContainsKey(c) ? dict[c] : c.ToString());
            }
            sentence = iteration.ToString();
            iteration = new StringBuilder();
        }
        Debug.Log(sentence);
    }
}
