//using Temple.Domain.Entities.DD.Dialogue;

//namespace Temple.ViewModel.DD.Dialogue;

//public static class DialogueDataFactory
//{
//    public static DialogueData GenerateDialogueData(
//        string npcId)
//    {
//        var dialogueData = new DialogueData();

//        switch (npcId)
//        {
//            case "innkeeper":
//                dialogueData.NPCPortraitPath = "DD/Images/Innkeeper.png";
//                break;
//            case "guard":
//                dialogueData.NPCPortraitPath = "DD/Images/Guard.jpg";
//                break;
//            case "captain":
//                dialogueData.NPCPortraitPath = "DD/Images/Captain.png";
//                break;
//            default:
//                throw new InvalidOperationException("Unknown npcId");
//        }

//        return dialogueData;
//    }
//}