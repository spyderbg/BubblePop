using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(RangedFloat), true)]
public class RangedFloatDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty( position, label, property );
        position = EditorGUI.PrefixLabel( position, label );

        var minProp = property.FindPropertyRelative( "minValue" );
        var maxProp = property.FindPropertyRelative( "maxValue" );

        var minValue = minProp.floatValue;
	    var maxValue = maxProp.floatValue;

        var rangeMin = 0.0f;
        var rangeMax = 1.0f;

        var ranges = (MinMaxRangeAttribute[]) fieldInfo.GetCustomAttributes( typeof( MinMaxRangeAttribute ), true );
        if( ranges.Length > 0 )
        {
            rangeMin = ranges[0].Min;
            rangeMax = ranges[0].Max;
        }

        const float rangeBoundsLabelWidth = 40.0f;

        var rangeBoundsLabel1Rect = new Rect( position );
        rangeBoundsLabel1Rect.width = rangeBoundsLabelWidth;
        GUI.Label( rangeBoundsLabel1Rect, new GUIContent(minValue.ToString("F2")) );
        position.xMin += rangeBoundsLabelWidth;

        var rangeBoundsLabel2Rect = new Rect(position);
        rangeBoundsLabel2Rect.xMin = rangeBoundsLabel2Rect.xMax - rangeBoundsLabelWidth;
        GUI.Label( rangeBoundsLabel2Rect, new GUIContent(maxValue.ToString("F2")) );
        position.xMax -= rangeBoundsLabelWidth;

        EditorGUI.BeginChangeCheck();
	    EditorGUI.MinMaxSlider( position, ref minValue, ref maxValue, rangeMin, rangeMax );
        if( EditorGUI.EndChangeCheck() )
        {
            minProp.floatValue = minValue;
            maxProp.floatValue = maxValue;
        }

        EditorGUI.EndProperty();
    }
}

#endif 
