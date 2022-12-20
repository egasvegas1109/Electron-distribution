using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Main : MonoBehaviour
{
    [SerializeField] GameObject electron;
    [SerializeField] Transform folder;
    [SerializeField] InputField input_n0, input_tn, input_D, input_L, input_h, input_tau, input_eps, input_ngr2, input_ngr3;
    [SerializeField] int n0 = 1000, ngr2 = 1100, ngr3 = 1050;
    [SerializeField] double tn = 0.0001, D = 0.01, L = 0.002, h = 0.0001, tau = 0.0000001, eps = 0.0001;

    public void Draw()
    {
        n0 = Convert.ToInt32(input_n0.text);
        tn = Convert.ToDouble(input_tn.text);
        D = Convert.ToDouble(input_D.text);
        L = Convert.ToDouble(input_L.text);
        h = Convert.ToDouble(input_h.text);
        tau = Convert.ToDouble(input_tau.text);
        eps = Convert.ToDouble(input_eps.text);
        ngr2 = Convert.ToInt32(input_ngr2.text);
        ngr3 = Convert.ToInt32(input_ngr3.text);

        while (folder.transform.childCount > 0)
        {
            DestroyImmediate(folder.transform.GetChild(0).gameObject);
        }

        int nX = (int)(L / h) + 1;
        int nY = (int)(L / h) + 1;

        double[,] Unew = new double[nX, nY];
        double[,] Uold = new double[nX, nY];
        float[,] Unity = new float[nX, nY];
        GameObject[,] masselec = new GameObject[nX, nY];

        for (int i = 0; i < nX; i++)
        {
            for (int j = 0; j < nY; j++)
            {
                Unew[i, j] = 0;
                Uold[i, j] = 0;
            }
        }

        //заданные границы
        for (int j = 0; j < nY; j++)
        {
            Uold[0, j] = ngr2;
        }

        for (int i = 0; i < nX; i++)
        {
            Uold[i, nY - 1] = ngr3;
        }

        bool stop = false;

        while (stop == false)
        {
            for (int i = 1; i < nX - 1; i++)
            {
                for (int j = 1; j < nY - 1; j++)
                {
                    Unew[i, j] = -(((Uold[i, j] - n0) * tau) / tn) + ((D * tau) / (h * h)) * (Uold[i - 1, j] + Uold[i + 1, j] + Uold[i, j - 1] + Uold[i, j + 1] - 4 * Uold[i, j]) + Uold[i, j];
                }
            }

            //граница 1
            for (int i = 1; i < nX - 1; i++)
            {
                Unew[i, 0] = Unew[i, 1];
            }

            //граница 4
            for (int j = 1; j < nY - 1; j++)
            {
                Unew[nX - 1, j] = Unew[nX - 2, j];
            }
            stop = true;

            for (int i = 1; i < nX - 1; i++)//проверка точности
            {
                for (int j = 1; j < nY - 1; j++)
                {
                    if (Math.Abs(Unew[i, j] - Uold[i, j]) > eps)
                        stop = false;
                }
            }

            //переписываем значения
            for (int i = 1; i < nX; i++)
            {
                for (int j = 0; j < nY - 1; j++)
                {
                    Uold[i, j] = Unew[i, j];
                }
            }
        }

        for (int i = 0; i < nX; i++)
        {
            for (int j = 0; j < nY; j++)
            {
                Unity[i, j] = (float)Math.Round(Uold[i, j]);
            }
        }

        for (int i = 0; i < nX; i++)
        {
            for (int j = 0; j < nY; j++)
            {
                GameObject elect = Instantiate(electron, new Vector3(i, j, 0), Quaternion.identity);
                elect.transform.SetParent(folder);
                elect.GetComponent<SpriteRenderer>().color = new Color((float)Math.Cos((Unity[i, j] /15) / 255f), (float)Math.Cos((Unity[i, j] / 5) / 255f), (float)Math.Cos((Unity[i, j] / 5) / 255f), 5f);
                elect.name = Convert.ToString(Unity[i, j]);
            }
        }
    }
}
