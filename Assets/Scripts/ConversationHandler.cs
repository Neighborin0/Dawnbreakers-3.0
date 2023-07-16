using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LabLine
{
    public string text;
    public string expression;
    public string unit;
    public float textSpeed;
}
public class TutorialConversationHandler : MonoBehaviour
{
    
    public static List<LabLine> DustyAureliaMeeting1 = new List<LabLine>
    {
          new LabLine { expression = "neutral_darkened", text = "Who goes there!?", unit = "DustyEnemy", textSpeed = 0.03f, },
          new LabLine { expression = "neutral_darkened", text = "Another accursed creature!? Have at you!", unit = "DustyEnemy", textSpeed = 0.03f, },
           new LabLine { expression = "neutral", text = "I do not wish to fight you. Just let me pass and I'll be on my way.",  unit = "Aurelia", textSpeed = 0.02f,},
            new LabLine { expression = "neutral_darkened", text = "You think me a fool!? I've been told that lie before and nearly lost my life!", unit = "DustyEnemy", textSpeed = 0.03f, },
             new LabLine { expression = "neutral_darkened", text = "You can't trick me this time monster!", unit = "DustyEnemy", textSpeed = 0.03f, },
              new LabLine { expression = "neutral", text = "If you insist on fighting so be it.", unit = "Aurelia", textSpeed = 0.03f, },
    };

    public static List<LabLine> DustyAureliaMeeting2 = new List<LabLine>
    {
         new LabLine { expression = "neutral_darkened", text = "That's enough.", unit = "DustyEnemy", textSpeed = 0.03f, },
         new LabLine { expression = "neutral_darkened", text = "There's something different about the way you fight. There's an intention in it I've haven't seen from any other monster in here.", unit = "DustyEnemy", textSpeed = 0.03f, },
         new LabLine { expression = "neutral", text = "Perhaps there is some truth to your words.", unit = "DustyEnemy", textSpeed = 0.03f,  },
         new LabLine { expression = "neutral", text = "If you can forgive me for the attack, I think proper introductions are in order.", unit = "DustyEnemy", textSpeed = 0.03f,  },
         new LabLine { expression = "neutral", text = "My name is Dusty, Commander of the Iron Wolves!" , unit = "DustyEnemy", textSpeed = 0.03f, },
         new LabLine { expression = "neutral", text = "May I have your name in turn?" , unit = "DustyEnemy" , textSpeed = 0.03f,},
          new LabLine { expression = "neutral", text = "...",  unit = "Aurelia", textSpeed = 0.04f,},
          new LabLine { expression = "neutral", text = "I'm sorry but I have a mission to complete. Just allow me to pass through and that will be the end of it.",  unit = "Aurelia", textSpeed = 0.02f, },
          new LabLine { expression = "neutral", text = "Hmm...", unit = "DustyEnemy", textSpeed = 0.03f, },
          new LabLine { expression = "neutral", text = "Very well.", unit = "DustyEnemy", textSpeed = 0.03f, },
    };

    public static List<LabLine> DustyAureliaPostMeeting = new List<LabLine>
    {
         new LabLine { expression = "neutral", text = "You insist on following me?", unit = "Aurelia", textSpeed = 0.03f, },
         new LabLine { expression = "neutral", text = "Listen. You're the only other person I've seen in this place. I cannot bear the thought of navigating this maze alone any longer. Especially without my men.", unit = "Dusty", textSpeed = 0.03f, },
         new LabLine { expression = "neutral", text = "Please, you seem to know this place better than I do. I need your help. If you need aid in battle I'll gladly offer my services.", unit = "Dusty", textSpeed = 0.03f, },
         new LabLine { expression = "neutral", text = "...", unit = "Aurelia", textSpeed = 0.03f, },
         new LabLine { expression = "neutral", text = "If you're going to follow me then stay close.", unit = "Aurelia", textSpeed = 0.03f, },
         new LabLine { expression = "neutral", text = "There are dangers lurking ahead.", unit = "Aurelia", textSpeed = 0.03f, },
    };

    public static List<LabLine> DustyAureliaRestMeeting = new List<LabLine>
    {
         new LabLine { expression = "neutral", text = "Here.", unit = "Aurelia", textSpeed = 0.03f, },
         new LabLine { expression = "neutral", text = "Why are we stopping?", unit = "Dusty", textSpeed = 0.03f, },
         new LabLine { expression = "neutral", text = "You need rest. I've seen your wounds, you can't continue fighting like that.", unit = "Aurelia", textSpeed = 0.03f, },
        new LabLine { expression = "neutral", text = "As if I could rest! Another monster could spring on us at any moment!", unit = "Dusty", textSpeed = 0.03f, },
         new LabLine { expression = "neutral", text = "No. Not here atleast. I was clearing out the area before meeting you. We'll be safe.", unit = "Aurelia", textSpeed = 0.03f, },
          new LabLine { expression = "neutral", text = "How could you be so sure!? Have you've seen how dark it is!? Anything could appear!", unit = "Dusty", textSpeed = 0.03f, },
           new LabLine { expression = "neutral", text = "You're going to have to trust me. If it puts you at ease, I'll stand watch.", unit = "Aurelia", textSpeed = 0.03f, },
           new LabLine { expression = "neutral", text = "At ease? I've been fighting ever since I've got here! My men are gone! There are abominations everywhere!", unit = "Dusty", textSpeed = 0.03f, },
             new LabLine { expression = "neutral", text = "I mu-", unit = "Dusty", textSpeed = 0.03f, },
              new LabLine { expression = "neutral", text = "Aughhh!", unit = "Dusty", textSpeed = 0.03f, },
                new LabLine { expression = "neutral", text = "I must...continue on...", unit = "Dusty", textSpeed = 0.03f, },
                 new LabLine { expression = "neutral", text = "...", unit = "Aurelia", textSpeed = 0.03f, },
                  new LabLine { expression = "neutral", text = "Listen to me Dusty. You're going to kill yourself if you continue like this. I've seen it happen before.", unit = "Aurelia", textSpeed = 0.03f, },
                   new LabLine { expression = "neutral", text = "You've come to me for aid, so allow me to help you.", unit = "Aurelia", textSpeed = 0.03f, },
                    new LabLine { expression = "neutral", text = "How can... I know...to trust you?", unit = "Dusty", textSpeed = 0.03f, },
                     new LabLine { expression = "neutral", text = "I could have killed you already, yet here you remain.", unit = "Aurelia", textSpeed = 0.03f, },
                      new LabLine { expression = "neutral", text = "I could have casted you aside and left, yet here I remain.", unit = "Aurelia", textSpeed = 0.03f, },
                       new LabLine { expression = "neutral", text = "Now we're here. I could kill you now in your weakened state.", unit = "Aurelia", textSpeed = 0.03f, },
                         new LabLine { expression = "neutral", text = "But I trust you Dusty. A man like you could never backstab anyone.", unit = "Aurelia", textSpeed = 0.03f, },
                          new LabLine { expression = "neutral", text = "If I could place my trust in you, can you do the same for me?", unit = "Aurelia", textSpeed = 0.03f, },
                           new LabLine { expression = "neutral", text = "Hmmph.", unit = "Dusty", textSpeed = 0.03f, },
                            new LabLine { expression = "neutral", text = "I'm too weary to argue anyways.", unit = "Dusty", textSpeed = 0.03f, },
                            new LabLine { expression = "neutral", text = "I'll rest here, but only for a moment!", unit = "Dusty", textSpeed = 0.03f, },
                             new LabLine { expression = "neutral", text = "Try anything strange and I'll show you what happens when you betray my trust!", unit = "Dusty", textSpeed = 0.03f, },


    };

    public static List<LabLine> PrologueEnding = new List<LabLine>
    {
          new LabLine { expression = "neutral", text = "<color=#00FFF5>You sucks, like god damn. </color>", unit = "Queen", textSpeed = 0.03f, },
          new LabLine { expression = "neutral", text = "I know...", unit = "Aurelia", textSpeed = 0.03f, },

    };
}

