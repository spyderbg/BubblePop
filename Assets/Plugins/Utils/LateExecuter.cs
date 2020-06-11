using System.Collections;
using System.Reflection;
using UnityEngine;
using System;
 
 namespace Utils {
     
 public class Executor
 {
     private readonly object _object;
     private  readonly MonoBehaviour _script;
     
     public Executor(object obj)
     {
         _object = obj;
         _script = _object as MonoBehaviour;
     }

     public InvokeId DelayExecute(float DelayInSeconds, Action<object[]> lambda, params object[] parameters)
     {
        return new InvokeId( _script.StartCoroutine(Delayed(DelayInSeconds, lambda, parameters)));
     }
     
     public InvokeId DelayExecute(float DelayInSeconds, string methodName, params object[] parameters)
     {
         foreach (MethodInfo method in _object.GetType().GetMethods())
         {
             if (method.Name == methodName)
                 return new InvokeId(_script.StartCoroutine(Delayed(DelayInSeconds, method, parameters)));
         }
         return null;
     }
     
     public InvokeId ConditionExecute(Func<bool> condition, string methodName, params object[] parameters)
     {
         foreach (MethodInfo method in _object.GetType().GetMethods())
         {
             if (method.Name == methodName)
                 return new InvokeId(_script.StartCoroutine(Delayed(condition, method, parameters)));
         }
         return null;
     }
     
     public InvokeId ConditionExecute(Func<bool> condition, Action<object[]> lambda, params object[] parameters)
     {
         return new InvokeId(_script.StartCoroutine(Delayed(condition, lambda, parameters)));
     }

     public void StopExecute(InvokeId id)
     {
         _script.StopCoroutine(id.coroutine);
     }
     
     IEnumerator Delayed(float DelayInSeconds, Action<object[]> lambda, params object[] parameters)
     {
         yield return new WaitForSeconds(DelayInSeconds);
         lambda.Invoke(parameters);
     }
     
     IEnumerator Delayed(float DelayInSeconds, MethodInfo method, params object[] parameters)
     {
         yield return new WaitForSeconds(DelayInSeconds);
         method.Invoke(_object, parameters);
     }
     
     IEnumerator Delayed(Func<bool> condition, Action<object[]> lambda, params object[] parameters)
     {
         yield return new WaitUntil(condition);
         lambda.Invoke(parameters);
     }
     
     IEnumerator Delayed(Func<bool> condition, MethodInfo method, params object[] parameters)
     {
         yield return new WaitUntil(condition);
         method.Invoke(_object, parameters);
     }
 }
 
 public class InvokeId
 {
     public readonly Coroutine coroutine;
     public InvokeId(Coroutine coroutine)
     {
         this.coroutine = coroutine;
     }
 }
 
 } // Utils