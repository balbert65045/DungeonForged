using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactPanel : MonoBehaviour
{

    public GameObject Panel;
    public GameObject ArtifactSpot;
    public GameObject Artifact;
    public Text ArtifactName;
    public Text ArtifactDescription;

    void HidePanel()
    {
        Panel.SetActive(false);
    }

    public void ShowArtifact()
    {
        Panel.SetActive(true);
        Artifact = FindObjectOfType<CardDatabase>().GetRandomArtifact(FindObjectOfType<NewGroupStorage>().MyGroupCardStorage[0].ArtifactsHolding());
        Artifact artifactComponent = Artifact.GetComponent<Artifact>();
        GameObject artifactOut = Instantiate(Artifact, ArtifactSpot.transform);
        artifactOut.transform.localPosition = Vector3.zero;
        artifactOut.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        ArtifactName.text = artifactComponent.Name;
        ArtifactDescription.text = artifactComponent.Description;
    }

    public void AddArtifact()
    {
        FindObjectOfType<NewGroupStorage>().AddArtifact(Artifact);
        FindObjectOfType<CCSSelectionButton>().AddArtifact(Artifact);
        HidePanel();
    }
}
