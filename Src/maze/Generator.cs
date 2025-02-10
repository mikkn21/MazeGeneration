public interface IGenerator {
    void Step();
    bool IsComplete { get; }
}