public interface IGenerator {
    void Step();

    void Reset(); 

    void Back(); 

    void Run();

    bool IsComplete { get; }
}