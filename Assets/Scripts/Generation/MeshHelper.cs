using System.Collections.Generic;
using UnityEngine;

public class MeshHelper : MonoBehaviour
{
    public static void CombineMeshes( GameObject gameObject)
    {
        Vector3 position = gameObject.transform.position;
        gameObject.transform.position = Vector3.zero;

        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 1;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        gameObject.transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        gameObject.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine, true, true);
        gameObject.transform.gameObject.SetActive(true);

        gameObject.transform.position = position;
    }
}