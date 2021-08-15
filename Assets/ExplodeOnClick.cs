using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ExplodeOnClick : MonoBehaviour
{
    private Camera Camera;
    [SerializeField]
    private ParticleSystem ParticleSystemPrefab;
    public int MaxHits = 25;
    public float Radius = 10f;
    public LayerMask HitLayer;
    public LayerMask BlockExplosionLayer;
    public int MaxDamage = 50;
    public int MinDamage = 10;
    public float ExplosiveForce;

    private Collider[] Hits;

    private void Awake()
    {
        Camera = GetComponent<Camera>();
        Hits = new Collider[MaxHits];
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Instantiate(ParticleSystemPrefab, hit.point, Quaternion.identity);
                int hits = Physics.OverlapSphereNonAlloc(hit.point, Radius, Hits, HitLayer);

                for (int i = 0; i < hits; i++)
                {
                    if (Hits[i].TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
                    {
                        float distance = Vector3.Distance(hit.point, Hits[i].transform.position);

                        if (!Physics.Raycast(hit.point, (Hits[i].transform.position - hit.point).normalized, distance, BlockExplosionLayer.value))
                        {
                            rigidbody.AddExplosionForce(ExplosiveForce, hit.point, Radius);
                            Debug.Log($"Would hit {rigidbody.name} for {Mathf.FloorToInt(Mathf.Lerp(MaxDamage, MinDamage, distance / Radius))}");
                        }
                    }
                }
            }
        }
    }
}
