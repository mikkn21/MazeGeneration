public interface IGenerator {
    void Step();

    void Restart(); 

    void Back(); 


    bool IsComplete { get; }

    bool CanUndo { get; }
}