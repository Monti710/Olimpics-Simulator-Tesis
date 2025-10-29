using UnityEngine;

// Aseguramos que este script esté en un objeto que tenga el componente Cloth.
[RequireComponent(typeof(Cloth))]
public class ClothWind : MonoBehaviour
{
    private Cloth clothComponent;

    void Awake()
    {
        // Obtenemos la referencia al componente Cloth.
        clothComponent = GetComponent<Cloth>();
    }

    void Update()
    {
        // Verificamos que nuestro WindManager exista en la escena.
        if (WindManager.instance != null)
        {
            // Tomamos los datos del viento del manager.
            Vector3 windForce = WindManager.instance.windDirection.normalized * WindManager.instance.windStrength;

            // Aplicamos esa fuerza a la propiedad "External Acceleration" del componente Cloth.
            // Esta propiedad simula una fuerza de viento constante.
            clothComponent.externalAcceleration = windForce;
        }
        else
        {
            // Si no hay viento, nos aseguramos de que no haya aceleración externa.
            clothComponent.externalAcceleration = Vector3.zero;
        }
    }
}