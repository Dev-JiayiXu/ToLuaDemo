using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class ResLoader : MonoBehaviour
{
    public static ResLoader Create()
    {
        var loader = new ResLoader();

        return loader;
    }
    public void Dispose()
    {

    }
}