﻿using Assets.Scripts.Api;
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSG;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Zenject;
using GizmoExercises.Lifetime.Services;

namespace Assets.Scripts.Infrastructure
{
    public class RemoteDataManager : IDataManager
    {
        private string address = "https://raw.githubusercontent.com/MakFrogz/HomeWork_2/master/Assets/data.json";

        [Inject]
        private ICoroutineService _coroutineService;

        public PlayerStatsModelDto LoadModelData()
        {
            throw new NotImplementedException();
        }

        public IPromise Save(PlayerStatsModelDto dto)
        {
            var p = new Promise();
            _coroutineService.RunCoroutine(SaveData(dto, (isSuccess, message) =>
            {
                if (isSuccess)
                {
                    Debug.Log(message);
                    p.Resolve();
                }
                else
                {
                    p.Reject(new Exception(message));
                }
            }));
            return p;
        }

        private IEnumerator SaveData(PlayerStatsModelDto dto, Action<bool, string> callback)
        {
            string dtoJson = JsonUtility.ToJson(dto);
            using(var uwr = UnityWebRequest.Post(address, dtoJson))
            {
                yield return uwr.SendWebRequest();
                var isSuccess = string.IsNullOrEmpty(uwr.error);
                if (isSuccess)
                {
                    callback?.Invoke(isSuccess, uwr.downloadHandler.text);
                }
                else
                {
                    callback?.Invoke(isSuccess, uwr.error);
                }
            }
        }
    }
}
