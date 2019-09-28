public interface IAnimal {
    float RegularMovementSpeed { get; }
    float HostileMovementSpeed { get; set; }
    float RetreatMovementSpeed { get; set; }
}