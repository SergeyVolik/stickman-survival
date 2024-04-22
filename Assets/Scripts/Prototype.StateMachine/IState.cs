namespace Prototype
{
    public interface IState
    {
        public bool IsActive { get; set; }
        public void Awake() { }
        public void Start() { }
        public void OnEnable() { }
        public void OnDisable() { }
        public void Update() { }
        public void FixedUpdate() { }
    }
}