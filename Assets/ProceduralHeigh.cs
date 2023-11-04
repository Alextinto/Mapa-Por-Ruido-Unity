using System.Collections.Generic;
using UnityEngine;

//BUENAS CAPO COMO ANDAMOS
//aqui trato de explicarte un poco como utilizar de manera basica un mapa de ruido 
//para que puedas utilizarlo en tus cosas si quieres.
//este ejemplo utilizo cubitos de minecraft, no estan ni un poco optimizados asi que deberia correr bien mal XDD
//para aplicar el mapa ruido, ve al componente de este codigo en el "Visor de Mapa", hacele click derecho, y aprieta "regenerar"
//o simplemente cambia este codigo para que cuando presiones una tecla se llame a la funcion UpdateHeight
//finalmente para ver los cambios, mueve por el editor de escena, me dio flojera hacer una camara XD
//suerte en tus cositas ^^

//PD: Copien el codigo, hace bien para el medio ambiente

public class ProceduralHeigh : MonoBehaviour
{
    //ANCHO Y ALTO DEL MAPA DE RUIDO GENERADO (SOLO FUNCIONA AL CREARSE POR PRIMERA VEZ)
    public int pixWidth;
    public int pixHeight;

    // EL ORIGEN DENTRO DEL MAPA DE RUIDO
    public float xOrg;
    public float yOrg;

    // ESCALA DEL MAPA
    public float scale = 1.0F;


    //prefab del cubito
    public GameObject cube;
    //"matriz" de cubitos
    public List<List<GameObject>> array;

    //textura generada (se puede elegir una manual de antes, como ejemplo colocar el among us)
    public Texture2D noiseTex;

    //si se setea una textura de antes, no es necesario actualizar el mapa
    public bool updateTex;

    private Color[] pix;
    private Renderer rend;

    void Start()
    {
        updateTex = false;
        array = new List<List<GameObject>>();  
        
        //creamos una matriz de cubitos, cada cubito es un pixel de la imagen
        for (int i = 0; i < pixWidth; i++)
        {
            array.Add(new List<GameObject>());
            for (int j = 0; j < pixHeight; j++)
            {
                 array[i].Add(Instantiate(cube, new Vector3(i, 0, j), Quaternion.identity));
            }
        }

        rend = GetComponent<Renderer>();

        //si no tenemos una imagen predefinida, creamos una y la actualizamos
        if (noiseTex == null)
        {
            updateTex = true;
            // Inicializamos la textura y un arreglo de colores para mantener los datos del mapa de ruido
            noiseTex = new Texture2D(pixWidth, pixHeight);
            pix = new Color[noiseTex.width * noiseTex.height];
        }

        rend.material.mainTexture = noiseTex;
    }

    void CalcNoise()
    {
        // Por cada pixel...
        float y = 0.0F;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                //creamos su cordenadas
                float xCoord = xOrg + x / noiseTex.width * scale;
                float yCoord = yOrg + y / noiseTex.height * scale;

                //obtenemos el valor de las coordenadas
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                //la guardamos temporalmente
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }

        // Copiamos los datos de los pixeles y luego la apolicamos
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    void Update()
    {
        if(updateTex)
            CalcNoise();
    }

    [ContextMenu("Regenerar")]
    void UpdateHeight()
    {
        //por cada cubito de la matriz
        for (int i = 0; i < array.Count; i++)
        {
            for (int j = 0; j < array[0].Count; j++)
            {
                //obtenemos la posicion del cubito
                Vector3 v = array[i][j].transform.position;
                //obtenemos el valor de su mapa de ruido
                float value = noiseTex.GetPixel(i, j).r;
                //en este caso lo llevamos a escala 0-10
                value = Mathf.RoundToInt(value * 10f);
                //cambiamos la altura del cubito
                v.y = value;
                //seteamos la posicion
                array[i][j].transform.position = v;
            }
        }
    }
}
