using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;


public static class AthenaPlayfabAPI
{
    public static async Task<GetUserDataResult> GetUserDataAsync(List<string> keys, string playfabId)
    {
        var tcs = new TaskCompletionSource<GetUserDataResult>();
        var request = new GetUserDataRequest()
        {
            PlayFabId = playfabId,
            Keys = keys
        };

        PlayFabClientAPI.GetUserData(request, 
        result =>
        {
            tcs.SetResult(result);
        }, 
        error =>
        {
            Debug.Log("Failed to get user data: " + error.GenerateErrorReport());
            tcs.SetResult(null);
        });
        
        return await tcs.Task;
    }

    public static async Task<UpdateUserDataResult> UpdateUserDataAsync(Dictionary<string, string> data)
    {
        var tcs = new TaskCompletionSource<UpdateUserDataResult>();
        var request = new UpdateUserDataRequest()
        {
            Data = data
        };

        PlayFabClientAPI.UpdateUserData(request,
        result =>
        {
            tcs.SetResult(result);
        },
        error =>
        {
            Debug.Log("Failed to update user data: " + error.GenerateErrorReport());
            tcs.SetResult(null);
        });
        return await tcs.Task;
    }
}

