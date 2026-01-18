using Temple.Domain.Entities.DD.Dialogue;

namespace Temple.ViewModel.DD.Dialogue;

public static class DialogueDataFactory
{
    public static DialogueData GenerateDialogueData(
        string npcId)
    {
        var dialogueData = new DialogueData();

        switch (npcId)
        {
            case "Innkeeper":
                dialogueData.NPCPortraitPath = "DD/Images/Innkeeper.png";
                break;
            case "Guard":
                dialogueData.NPCPortraitPath = "DD/Images/Guard.jpg";
                break;
            case "Captain":
                dialogueData.NPCPortraitPath = "DD/Images/Captain.png";
                break;
        }

        return dialogueData;
    }
}