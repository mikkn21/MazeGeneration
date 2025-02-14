public interface IGenerator {
    void Step();

    void Reset(); 

    void Back(); 


    bool IsComplete { get; }
}