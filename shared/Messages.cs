namespace AnimalCrossing.Shared;

using System.Runtime.Serialization;

public enum MessageTypes {
    Intro = 0,
}

public interface IMessage : ISerializable {
    MessageTypes type { get; set; }
}

[Serializable]
public class MessageIntro : IMessage {

}