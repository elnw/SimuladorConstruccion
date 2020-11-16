using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using UnityEngine.UI;
using UnityEditor;
using System;
public class HomeController : MonoBehaviour
{
    private static Vector3 posicionInicial = new Vector3 { x = 2.12f, y = 4.03f, z = -2 };
    private static Vector3 escalaViga = new Vector3 { x = 2f, y = 0.4279298f, z = 1f };
    private List<GameObject> viga;
    private static string baseUrl = "https://iaropdecaa.herokuapp.com/api/v1/";
    private static string recursoViga = "viga";
    // Start is called before the first frame update

    private void Awake()
    {
        viga = new List<GameObject>(6);
    }

    void Start()
    {
        for(int i = 0; i < 6; i++)
        {
            var nuevoCubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nuevoCubo.transform.localScale = escalaViga;
            nuevoCubo.transform.position = new Vector3 { x = posicionInicial.x + escalaViga.x * i, y = posicionInicial.y, z = posicionInicial.z };

            if(i == 2 || i == 3)
            {
                var pesoRB = GameObject.Find("Peso").GetComponent<Rigidbody>();

                nuevoCubo.AddComponent<Rigidbody>().isKinematic = true;
                nuevoCubo.AddComponent<CharacterJoint>();
                nuevoCubo.GetComponent<CharacterJoint>().connectedBody = pesoRB;
            }

            viga.Add(nuevoCubo);
        }
    }

    public void MostrarDeformacion()
    {
        //Descomentar solo para ver los bloques por colores
        /*foreach(var item in viga)
        {
            item.GetComponent<MeshRenderer>().material.color = new Color(UnityEngine.Random.Range(0f, 255f) / 255, UnityEngine.Random.Range(0f, 255f) / 255, UnityEngine.Random.Range(0f, 255f) / 255);
        }*/

        InputField baseIngresada, alturaIngresada;

        baseIngresada = GameObject.Find("BaseInputField").GetComponent<InputField>();
        alturaIngresada = GameObject.Find("AlturaInputField").GetComponent<InputField>();
        string url = baseUrl + recursoViga + "?baseViga=" + baseIngresada.text + "&" + "alturaViga=" + alturaIngresada.text;
        RestClient.Get(url)
            .Then(respuesta =>
            {
                List<double> desplazamiento = new List<double>();
                
                string item = "";
                foreach(char letra in respuesta.Text)
                {
                    
                    if (letra == '-' || letra == '.' || char.IsNumber(letra))
                    {
                        
                        item += letra;
                    }
                    else
                    {
                        if(letra == ']')
                        {
                            
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                desplazamiento.Add(Convert.ToDouble(item));
                                
                            }
                            
                            item = "";
                        }
                    }
                }

                if(desplazamiento.Count > 0 && viga.Count == desplazamiento.Count)
                {
                    float factorDesplazamiento = 0.0f;

                    foreach(double despItem in desplazamiento)
                    {
                        factorDesplazamiento += Convert.ToSingle(despItem);
                    }
                    factorDesplazamiento /= 1e15f;

                    viga[1].transform.position = new Vector3(viga[1].transform.position.x,
                                                        viga[1].transform.position.y + (factorDesplazamiento) / 256,
                                                        viga[1].transform.position.z);

                    viga[2].transform.position = new Vector3(viga[2].transform.position.x,
                                                        viga[2].transform.position.y + (factorDesplazamiento) / 128,
                                                        viga[2].transform.position.z);

                    viga[3].transform.position = new Vector3(viga[3].transform.position.x,
                                                        viga[3].transform.position.y + (factorDesplazamiento) / 128,
                                                        viga[3].transform.position.z);

                    viga[4].transform.position = new Vector3(viga[4].transform.position.x,
                                                        viga[4].transform.position.y + (factorDesplazamiento) / 256,
                                                        viga[4].transform.position.z);
                }
                else
                {
                    EditorUtility.DisplayDialog("respuesta", "Debe ingresar medidas validas", "ok");
                }



                
            });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
