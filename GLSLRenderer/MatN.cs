// -----------------------------------------------------------------------
//  <copyright file="MatN.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Text;

namespace GLSLRenderer;

public class MatN<T> where T : VectorBase, new()
{
    protected readonly T[] m_columns;

    protected MatN(params T[] columns)
    {
        m_columns = columns.Select(o => new T { Components = o.Components.ToArray() }).ToArray();
    }
    
    public T this[in int i]
    {
        get => m_columns[i];
        set => m_columns[i] = new T { Components = value.Components.ToArray() };
    }
    
    // Overloads.
    public override string ToString()
    {
        var rowCount = m_columns.First().Components.Length;
        var colCount = m_columns.Length;

        var sb = new StringBuilder();
        for (var row = 0; row < rowCount; row++)
        {
            sb.Append('|');

            for (var col = 0; col < colCount; col++)
                sb.Append($" {m_columns[col][row],7:0.000}");

            sb.AppendLine(" |");
        }

        return sb.ToString().Trim('\n', '\r');
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return m_columns.SequenceEqual(((MatN<T>)obj).m_columns);
    }

    public override int GetHashCode() => m_columns.GetHashCode();
}