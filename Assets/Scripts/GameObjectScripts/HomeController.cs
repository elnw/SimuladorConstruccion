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
    private static string baseUrl = "http://127.0.0.1:5000/api/v1/";
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

    public Vector3 GetCurvePoint(Vector3 a, Vector3 b, Vector3 c, float t) {
        Vector3 p1 = Vector3.Lerp(a, b, t);
        Vector3 p2 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p1, p2, t);
    }

    public void MostrarDeformacion()
    {
        //Descomentar solo para ver los bloques por colores
        /* foreach(var item in viga)
        {
            item.GetComponent<MeshRenderer>().material.color = new Color(UnityEngine.Random.Range(0f, 255f) / 255, UnityEngine.Random.Range(0f, 255f) / 255, UnityEngine.Random.Range(0f, 255f) / 255);
        } */

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
                    //EditorUtility.DisplayDialog ("Error", factorDesplazamiento.ToString(), "Ok");
                    //factorDesplazamiento /= 1e15f;
                    // EditorUtility.DisplayDialog ("Error", factorDesplazamiento.ToString(), "Ok");

                    viga[1].transform.position = new Vector3(viga[1].transform.position.x,
                                                        viga[1].transform.position.y + factorDesplazamiento * 90,
                                                        viga[1].transform.position.z);

                    viga[2].transform.position = new Vector3(viga[2].transform.position.x,
                                                        viga[2].transform.position.y + factorDesplazamiento * 110 ,
                                                        viga[2].transform.position.z);

                    viga[3].transform.position = new Vector3(viga[3].transform.position.x,
                                                        viga[3].transform.position.y + factorDesplazamiento * 130,
                                                        viga[3].transform.position.z);

                    viga[4].transform.position = new Vector3(viga[4].transform.position.x,
                                                        viga[4].transform.position.y + factorDesplazamiento * 110,
                                                        viga[4].transform.position.z);
                    
                    //Dibujar linea
                    viga[2].AddComponent<LineRenderer>();
                    List<Vector3> linePositions = new List<Vector3>();
                    LineRenderer lineaCurva = viga[2].GetComponent<LineRenderer>();

                    var punto1 = viga[2].transform.position;
                    var punto2 = viga[3].transform.position;
                    var punto3 = viga[4].transform.position;

                    punto1 = new Vector3(punto1.x - 1f,punto1.y - 0.1f ,punto1.z - 1 );
                    punto2 = new Vector3(punto2.x,punto2.y - 0.1f ,punto2.z - 1);
                    punto3 = new Vector3(punto3.x,punto3.y - 0.1f ,punto3.z - 1);
                    

                    for (float t = 0; t <= 1; t += 0.1f) {
                        linePositions.Add(GetCurvePoint(punto1, punto2, punto3, t));
                    }
                    lineaCurva.positionCount = linePositions.Count;
                    lineaCurva.SetPositions(linePositions.ToArray());
                    lineaCurva.startWidth = 0.2f;
                    lineaCurva.endWidth = 0.2f;
                    lineaCurva.startColor = Color.red;
                    lineaCurva.endColor = Color.blue;
                    lineaCurva.enabled = true;

                }
                else
                {
                    EditorUtility.DisplayDialog("respuesta", "Debe ingresar medidas validas", "ok");
                }

                //EditorUtility.DisplayDialog ("Error", "Terminé", "Ok");

                
            }).Catch(err => EditorUtility.DisplayDialog ("Error", err.Message, "Ok"));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
