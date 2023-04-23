// -----------------------------------------------------------------------
//  <copyright file="VectorBase.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace Transpiler;

public partial class VectorBase
{
    // Swizzles, swizzles, everywhere...
    public vec2 xx { get => new(x, x); set { x = value.x; x = value.y; } }
    public vec2 xy { get => new(x, y); set { x = value.x; y = value.y; } }
    public vec2 xz { get => new(x, z); set { x = value.x; z = value.y; } }
    public vec2 xw { get => new(x, w); set { x = value.x; w = value.y; } }
    public vec2 yx { get => new(y, x); set { y = value.x; x = value.y; } }
    public vec2 yy { get => new(y, y); set { y = value.x; y = value.y; } }
    public vec2 yz { get => new(y, z); set { y = value.x; z = value.y; } }
    public vec2 yw { get => new(y, w); set { y = value.x; w = value.y; } }
    public vec2 zx { get => new(z, x); set { z = value.x; x = value.y; } }
    public vec2 zy { get => new(z, y); set { z = value.x; y = value.y; } }
    public vec2 zz { get => new(z, z); set { z = value.x; z = value.y; } }
    public vec2 zw { get => new(z, w); set { z = value.x; w = value.y; } }
    public vec2 wx { get => new(w, x); set { w = value.x; x = value.y; } }
    public vec2 wy { get => new(w, y); set { w = value.x; y = value.y; } }
    public vec2 wz { get => new(w, z); set { w = value.x; z = value.y; } }
    public vec2 ww { get => new(w, w); set { w = value.x; w = value.y; } }
    public vec2 rr { get => new(r, r); set { r = value.x; r = value.y; } }
    public vec2 rg { get => new(r, g); set { r = value.x; g = value.y; } }
    public vec2 rb { get => new(r, b); set { r = value.x; b = value.y; } }
    public vec2 ra { get => new(r, a); set { r = value.x; a = value.y; } }
    public vec2 gr { get => new(g, r); set { g = value.x; r = value.y; } }
    public vec2 gg { get => new(g, g); set { g = value.x; g = value.y; } }
    public vec2 gb { get => new(g, b); set { g = value.x; b = value.y; } }
    public vec2 ga { get => new(g, a); set { g = value.x; a = value.y; } }
    public vec2 br { get => new(b, r); set { b = value.x; r = value.y; } }
    public vec2 bg { get => new(b, g); set { b = value.x; g = value.y; } }
    public vec2 bb { get => new(b, b); set { b = value.x; b = value.y; } }
    public vec2 ba { get => new(b, a); set { b = value.x; a = value.y; } }
    public vec2 ar { get => new(a, r); set { a = value.x; r = value.y; } }
    public vec2 ag { get => new(a, g); set { a = value.x; g = value.y; } }
    public vec2 ab { get => new(a, b); set { a = value.x; b = value.y; } }
    public vec2 aa { get => new(a, a); set { a = value.x; a = value.y; } }
    public vec2 ss { get => new(s, s); set { s = value.x; s = value.y; } }
    public vec2 st { get => new(s, t); set { s = value.x; t = value.y; } }
    public vec2 sp { get => new(s, p); set { s = value.x; p = value.y; } }
    public vec2 sq { get => new(s, q); set { s = value.x; q = value.y; } }
    public vec2 ts { get => new(t, s); set { t = value.x; s = value.y; } }
    public vec2 tt { get => new(t, t); set { t = value.x; t = value.y; } }
    public vec2 tp { get => new(t, p); set { t = value.x; p = value.y; } }
    public vec2 tq { get => new(t, q); set { t = value.x; q = value.y; } }
    public vec2 ps { get => new(p, s); set { p = value.x; s = value.y; } }
    public vec2 pt { get => new(p, t); set { p = value.x; t = value.y; } }
    public vec2 pp { get => new(p, p); set { p = value.x; p = value.y; } }
    public vec2 pq { get => new(p, q); set { p = value.x; q = value.y; } }
    public vec2 qs { get => new(q, s); set { q = value.x; s = value.y; } }
    public vec2 qt { get => new(q, t); set { q = value.x; t = value.y; } }
    public vec2 qp { get => new(q, p); set { q = value.x; p = value.y; } }
    public vec2 qq { get => new(q, q); set { q = value.x; q = value.y; } }
    public vec3 xxx { get => new(x, x, x); set { x = value.x; x = value.y; x = value.z; } }
    public vec3 xxy { get => new(x, x, y); set { x = value.x; x = value.y; y = value.z; } }
    public vec3 xxz { get => new(x, x, z); set { x = value.x; x = value.y; z = value.z; } }
    public vec3 xxw { get => new(x, x, w); set { x = value.x; x = value.y; w = value.z; } }
    public vec3 xyx { get => new(x, y, x); set { x = value.x; y = value.y; x = value.z; } }
    public vec3 xyy { get => new(x, y, y); set { x = value.x; y = value.y; y = value.z; } }
    public vec3 xyz { get => new(x, y, z); set { x = value.x; y = value.y; z = value.z; } }
    public vec3 xyw { get => new(x, y, w); set { x = value.x; y = value.y; w = value.z; } }
    public vec3 xzx { get => new(x, z, x); set { x = value.x; z = value.y; x = value.z; } }
    public vec3 xzy { get => new(x, z, y); set { x = value.x; z = value.y; y = value.z; } }
    public vec3 xzz { get => new(x, z, z); set { x = value.x; z = value.y; z = value.z; } }
    public vec3 xzw { get => new(x, z, w); set { x = value.x; z = value.y; w = value.z; } }
    public vec3 xwx { get => new(x, w, x); set { x = value.x; w = value.y; x = value.z; } }
    public vec3 xwy { get => new(x, w, y); set { x = value.x; w = value.y; y = value.z; } }
    public vec3 xwz { get => new(x, w, z); set { x = value.x; w = value.y; z = value.z; } }
    public vec3 xww { get => new(x, w, w); set { x = value.x; w = value.y; w = value.z; } }
    public vec3 yxx { get => new(y, x, x); set { y = value.x; x = value.y; x = value.z; } }
    public vec3 yxy { get => new(y, x, y); set { y = value.x; x = value.y; y = value.z; } }
    public vec3 yxz { get => new(y, x, z); set { y = value.x; x = value.y; z = value.z; } }
    public vec3 yxw { get => new(y, x, w); set { y = value.x; x = value.y; w = value.z; } }
    public vec3 yyx { get => new(y, y, x); set { y = value.x; y = value.y; x = value.z; } }
    public vec3 yyy { get => new(y, y, y); set { y = value.x; y = value.y; y = value.z; } }
    public vec3 yyz { get => new(y, y, z); set { y = value.x; y = value.y; z = value.z; } }
    public vec3 yyw { get => new(y, y, w); set { y = value.x; y = value.y; w = value.z; } }
    public vec3 yzx { get => new(y, z, x); set { y = value.x; z = value.y; x = value.z; } }
    public vec3 yzy { get => new(y, z, y); set { y = value.x; z = value.y; y = value.z; } }
    public vec3 yzz { get => new(y, z, z); set { y = value.x; z = value.y; z = value.z; } }
    public vec3 yzw { get => new(y, z, w); set { y = value.x; z = value.y; w = value.z; } }
    public vec3 ywx { get => new(y, w, x); set { y = value.x; w = value.y; x = value.z; } }
    public vec3 ywy { get => new(y, w, y); set { y = value.x; w = value.y; y = value.z; } }
    public vec3 ywz { get => new(y, w, z); set { y = value.x; w = value.y; z = value.z; } }
    public vec3 yww { get => new(y, w, w); set { y = value.x; w = value.y; w = value.z; } }
    public vec3 zxx { get => new(z, x, x); set { z = value.x; x = value.y; x = value.z; } }
    public vec3 zxy { get => new(z, x, y); set { z = value.x; x = value.y; y = value.z; } }
    public vec3 zxz { get => new(z, x, z); set { z = value.x; x = value.y; z = value.z; } }
    public vec3 zxw { get => new(z, x, w); set { z = value.x; x = value.y; w = value.z; } }
    public vec3 zyx { get => new(z, y, x); set { z = value.x; y = value.y; x = value.z; } }
    public vec3 zyy { get => new(z, y, y); set { z = value.x; y = value.y; y = value.z; } }
    public vec3 zyz { get => new(z, y, z); set { z = value.x; y = value.y; z = value.z; } }
    public vec3 zyw { get => new(z, y, w); set { z = value.x; y = value.y; w = value.z; } }
    public vec3 zzx { get => new(z, z, x); set { z = value.x; z = value.y; x = value.z; } }
    public vec3 zzy { get => new(z, z, y); set { z = value.x; z = value.y; y = value.z; } }
    public vec3 zzz { get => new(z, z, z); set { z = value.x; z = value.y; z = value.z; } }
    public vec3 zzw { get => new(z, z, w); set { z = value.x; z = value.y; w = value.z; } }
    public vec3 zwx { get => new(z, w, x); set { z = value.x; w = value.y; x = value.z; } }
    public vec3 zwy { get => new(z, w, y); set { z = value.x; w = value.y; y = value.z; } }
    public vec3 zwz { get => new(z, w, z); set { z = value.x; w = value.y; z = value.z; } }
    public vec3 zww { get => new(z, w, w); set { z = value.x; w = value.y; w = value.z; } }
    public vec3 wxx { get => new(w, x, x); set { w = value.x; x = value.y; x = value.z; } }
    public vec3 wxy { get => new(w, x, y); set { w = value.x; x = value.y; y = value.z; } }
    public vec3 wxz { get => new(w, x, z); set { w = value.x; x = value.y; z = value.z; } }
    public vec3 wxw { get => new(w, x, w); set { w = value.x; x = value.y; w = value.z; } }
    public vec3 wyx { get => new(w, y, x); set { w = value.x; y = value.y; x = value.z; } }
    public vec3 wyy { get => new(w, y, y); set { w = value.x; y = value.y; y = value.z; } }
    public vec3 wyz { get => new(w, y, z); set { w = value.x; y = value.y; z = value.z; } }
    public vec3 wyw { get => new(w, y, w); set { w = value.x; y = value.y; w = value.z; } }
    public vec3 wzx { get => new(w, z, x); set { w = value.x; z = value.y; x = value.z; } }
    public vec3 wzy { get => new(w, z, y); set { w = value.x; z = value.y; y = value.z; } }
    public vec3 wzz { get => new(w, z, z); set { w = value.x; z = value.y; z = value.z; } }
    public vec3 wzw { get => new(w, z, w); set { w = value.x; z = value.y; w = value.z; } }
    public vec3 wwx { get => new(w, w, x); set { w = value.x; w = value.y; x = value.z; } }
    public vec3 wwy { get => new(w, w, y); set { w = value.x; w = value.y; y = value.z; } }
    public vec3 wwz { get => new(w, w, z); set { w = value.x; w = value.y; z = value.z; } }
    public vec3 www { get => new(w, w, w); set { w = value.x; w = value.y; w = value.z; } }
    public vec3 rrr { get => new(r, r, r); set { r = value.x; r = value.y; r = value.z; } }
    public vec3 rrg { get => new(r, r, g); set { r = value.x; r = value.y; g = value.z; } }
    public vec3 rrb { get => new(r, r, b); set { r = value.x; r = value.y; b = value.z; } }
    public vec3 rra { get => new(r, r, a); set { r = value.x; r = value.y; a = value.z; } }
    public vec3 rgr { get => new(r, g, r); set { r = value.x; g = value.y; r = value.z; } }
    public vec3 rgg { get => new(r, g, g); set { r = value.x; g = value.y; g = value.z; } }
    public vec3 rgb { get => new(r, g, b); set { r = value.x; g = value.y; b = value.z; } }
    public vec3 rga { get => new(r, g, a); set { r = value.x; g = value.y; a = value.z; } }
    public vec3 rbr { get => new(r, b, r); set { r = value.x; b = value.y; r = value.z; } }
    public vec3 rbg { get => new(r, b, g); set { r = value.x; b = value.y; g = value.z; } }
    public vec3 rbb { get => new(r, b, b); set { r = value.x; b = value.y; b = value.z; } }
    public vec3 rba { get => new(r, b, a); set { r = value.x; b = value.y; a = value.z; } }
    public vec3 rar { get => new(r, a, r); set { r = value.x; a = value.y; r = value.z; } }
    public vec3 rag { get => new(r, a, g); set { r = value.x; a = value.y; g = value.z; } }
    public vec3 rab { get => new(r, a, b); set { r = value.x; a = value.y; b = value.z; } }
    public vec3 raa { get => new(r, a, a); set { r = value.x; a = value.y; a = value.z; } }
    public vec3 grr { get => new(g, r, r); set { g = value.x; r = value.y; r = value.z; } }
    public vec3 grg { get => new(g, r, g); set { g = value.x; r = value.y; g = value.z; } }
    public vec3 grb { get => new(g, r, b); set { g = value.x; r = value.y; b = value.z; } }
    public vec3 gra { get => new(g, r, a); set { g = value.x; r = value.y; a = value.z; } }
    public vec3 ggr { get => new(g, g, r); set { g = value.x; g = value.y; r = value.z; } }
    public vec3 ggg { get => new(g, g, g); set { g = value.x; g = value.y; g = value.z; } }
    public vec3 ggb { get => new(g, g, b); set { g = value.x; g = value.y; b = value.z; } }
    public vec3 gga { get => new(g, g, a); set { g = value.x; g = value.y; a = value.z; } }
    public vec3 gbr { get => new(g, b, r); set { g = value.x; b = value.y; r = value.z; } }
    public vec3 gbg { get => new(g, b, g); set { g = value.x; b = value.y; g = value.z; } }
    public vec3 gbb { get => new(g, b, b); set { g = value.x; b = value.y; b = value.z; } }
    public vec3 gba { get => new(g, b, a); set { g = value.x; b = value.y; a = value.z; } }
    public vec3 gar { get => new(g, a, r); set { g = value.x; a = value.y; r = value.z; } }
    public vec3 gag { get => new(g, a, g); set { g = value.x; a = value.y; g = value.z; } }
    public vec3 gab { get => new(g, a, b); set { g = value.x; a = value.y; b = value.z; } }
    public vec3 gaa { get => new(g, a, a); set { g = value.x; a = value.y; a = value.z; } }
    public vec3 brr { get => new(b, r, r); set { b = value.x; r = value.y; r = value.z; } }
    public vec3 brg { get => new(b, r, g); set { b = value.x; r = value.y; g = value.z; } }
    public vec3 brb { get => new(b, r, b); set { b = value.x; r = value.y; b = value.z; } }
    public vec3 bra { get => new(b, r, a); set { b = value.x; r = value.y; a = value.z; } }
    public vec3 bgr { get => new(b, g, r); set { b = value.x; g = value.y; r = value.z; } }
    public vec3 bgg { get => new(b, g, g); set { b = value.x; g = value.y; g = value.z; } }
    public vec3 bgb { get => new(b, g, b); set { b = value.x; g = value.y; b = value.z; } }
    public vec3 bga { get => new(b, g, a); set { b = value.x; g = value.y; a = value.z; } }
    public vec3 bbr { get => new(b, b, r); set { b = value.x; b = value.y; r = value.z; } }
    public vec3 bbg { get => new(b, b, g); set { b = value.x; b = value.y; g = value.z; } }
    public vec3 bbb { get => new(b, b, b); set { b = value.x; b = value.y; b = value.z; } }
    public vec3 bba { get => new(b, b, a); set { b = value.x; b = value.y; a = value.z; } }
    public vec3 bar { get => new(b, a, r); set { b = value.x; a = value.y; r = value.z; } }
    public vec3 bag { get => new(b, a, g); set { b = value.x; a = value.y; g = value.z; } }
    public vec3 bab { get => new(b, a, b); set { b = value.x; a = value.y; b = value.z; } }
    public vec3 baa { get => new(b, a, a); set { b = value.x; a = value.y; a = value.z; } }
    public vec3 arr { get => new(a, r, r); set { a = value.x; r = value.y; r = value.z; } }
    public vec3 arg { get => new(a, r, g); set { a = value.x; r = value.y; g = value.z; } }
    public vec3 arb { get => new(a, r, b); set { a = value.x; r = value.y; b = value.z; } }
    public vec3 ara { get => new(a, r, a); set { a = value.x; r = value.y; a = value.z; } }
    public vec3 agr { get => new(a, g, r); set { a = value.x; g = value.y; r = value.z; } }
    public vec3 agg { get => new(a, g, g); set { a = value.x; g = value.y; g = value.z; } }
    public vec3 agb { get => new(a, g, b); set { a = value.x; g = value.y; b = value.z; } }
    public vec3 aga { get => new(a, g, a); set { a = value.x; g = value.y; a = value.z; } }
    public vec3 abr { get => new(a, b, r); set { a = value.x; b = value.y; r = value.z; } }
    public vec3 abg { get => new(a, b, g); set { a = value.x; b = value.y; g = value.z; } }
    public vec3 abb { get => new(a, b, b); set { a = value.x; b = value.y; b = value.z; } }
    public vec3 aba { get => new(a, b, a); set { a = value.x; b = value.y; a = value.z; } }
    public vec3 aar { get => new(a, a, r); set { a = value.x; a = value.y; r = value.z; } }
    public vec3 aag { get => new(a, a, g); set { a = value.x; a = value.y; g = value.z; } }
    public vec3 aab { get => new(a, a, b); set { a = value.x; a = value.y; b = value.z; } }
    public vec3 aaa { get => new(a, a, a); set { a = value.x; a = value.y; a = value.z; } }
    public vec3 sss { get => new(s, s, s); set { s = value.x; s = value.y; s = value.z; } }
    public vec3 sst { get => new(s, s, t); set { s = value.x; s = value.y; t = value.z; } }
    public vec3 ssp { get => new(s, s, p); set { s = value.x; s = value.y; p = value.z; } }
    public vec3 ssq { get => new(s, s, q); set { s = value.x; s = value.y; q = value.z; } }
    public vec3 sts { get => new(s, t, s); set { s = value.x; t = value.y; s = value.z; } }
    public vec3 stt { get => new(s, t, t); set { s = value.x; t = value.y; t = value.z; } }
    public vec3 stp { get => new(s, t, p); set { s = value.x; t = value.y; p = value.z; } }
    public vec3 stq { get => new(s, t, q); set { s = value.x; t = value.y; q = value.z; } }
    public vec3 sps { get => new(s, p, s); set { s = value.x; p = value.y; s = value.z; } }
    public vec3 spt { get => new(s, p, t); set { s = value.x; p = value.y; t = value.z; } }
    public vec3 spp { get => new(s, p, p); set { s = value.x; p = value.y; p = value.z; } }
    public vec3 spq { get => new(s, p, q); set { s = value.x; p = value.y; q = value.z; } }
    public vec3 sqs { get => new(s, q, s); set { s = value.x; q = value.y; s = value.z; } }
    public vec3 sqt { get => new(s, q, t); set { s = value.x; q = value.y; t = value.z; } }
    public vec3 sqp { get => new(s, q, p); set { s = value.x; q = value.y; p = value.z; } }
    public vec3 sqq { get => new(s, q, q); set { s = value.x; q = value.y; q = value.z; } }
    public vec3 tss { get => new(t, s, s); set { t = value.x; s = value.y; s = value.z; } }
    public vec3 tst { get => new(t, s, t); set { t = value.x; s = value.y; t = value.z; } }
    public vec3 tsp { get => new(t, s, p); set { t = value.x; s = value.y; p = value.z; } }
    public vec3 tsq { get => new(t, s, q); set { t = value.x; s = value.y; q = value.z; } }
    public vec3 tts { get => new(t, t, s); set { t = value.x; t = value.y; s = value.z; } }
    public vec3 ttt { get => new(t, t, t); set { t = value.x; t = value.y; t = value.z; } }
    public vec3 ttp { get => new(t, t, p); set { t = value.x; t = value.y; p = value.z; } }
    public vec3 ttq { get => new(t, t, q); set { t = value.x; t = value.y; q = value.z; } }
    public vec3 tps { get => new(t, p, s); set { t = value.x; p = value.y; s = value.z; } }
    public vec3 tpt { get => new(t, p, t); set { t = value.x; p = value.y; t = value.z; } }
    public vec3 tpp { get => new(t, p, p); set { t = value.x; p = value.y; p = value.z; } }
    public vec3 tpq { get => new(t, p, q); set { t = value.x; p = value.y; q = value.z; } }
    public vec3 tqs { get => new(t, q, s); set { t = value.x; q = value.y; s = value.z; } }
    public vec3 tqt { get => new(t, q, t); set { t = value.x; q = value.y; t = value.z; } }
    public vec3 tqp { get => new(t, q, p); set { t = value.x; q = value.y; p = value.z; } }
    public vec3 tqq { get => new(t, q, q); set { t = value.x; q = value.y; q = value.z; } }
    public vec3 pss { get => new(p, s, s); set { p = value.x; s = value.y; s = value.z; } }
    public vec3 pst { get => new(p, s, t); set { p = value.x; s = value.y; t = value.z; } }
    public vec3 psp { get => new(p, s, p); set { p = value.x; s = value.y; p = value.z; } }
    public vec3 psq { get => new(p, s, q); set { p = value.x; s = value.y; q = value.z; } }
    public vec3 pts { get => new(p, t, s); set { p = value.x; t = value.y; s = value.z; } }
    public vec3 ptt { get => new(p, t, t); set { p = value.x; t = value.y; t = value.z; } }
    public vec3 ptp { get => new(p, t, p); set { p = value.x; t = value.y; p = value.z; } }
    public vec3 ptq { get => new(p, t, q); set { p = value.x; t = value.y; q = value.z; } }
    public vec3 pps { get => new(p, p, s); set { p = value.x; p = value.y; s = value.z; } }
    public vec3 ppt { get => new(p, p, t); set { p = value.x; p = value.y; t = value.z; } }
    public vec3 ppp { get => new(p, p, p); set { p = value.x; p = value.y; p = value.z; } }
    public vec3 ppq { get => new(p, p, q); set { p = value.x; p = value.y; q = value.z; } }
    public vec3 pqs { get => new(p, q, s); set { p = value.x; q = value.y; s = value.z; } }
    public vec3 pqt { get => new(p, q, t); set { p = value.x; q = value.y; t = value.z; } }
    public vec3 pqp { get => new(p, q, p); set { p = value.x; q = value.y; p = value.z; } }
    public vec3 pqq { get => new(p, q, q); set { p = value.x; q = value.y; q = value.z; } }
    public vec3 qss { get => new(q, s, s); set { q = value.x; s = value.y; s = value.z; } }
    public vec3 qst { get => new(q, s, t); set { q = value.x; s = value.y; t = value.z; } }
    public vec3 qsp { get => new(q, s, p); set { q = value.x; s = value.y; p = value.z; } }
    public vec3 qsq { get => new(q, s, q); set { q = value.x; s = value.y; q = value.z; } }
    public vec3 qts { get => new(q, t, s); set { q = value.x; t = value.y; s = value.z; } }
    public vec3 qtt { get => new(q, t, t); set { q = value.x; t = value.y; t = value.z; } }
    public vec3 qtp { get => new(q, t, p); set { q = value.x; t = value.y; p = value.z; } }
    public vec3 qtq { get => new(q, t, q); set { q = value.x; t = value.y; q = value.z; } }
    public vec3 qps { get => new(q, p, s); set { q = value.x; p = value.y; s = value.z; } }
    public vec3 qpt { get => new(q, p, t); set { q = value.x; p = value.y; t = value.z; } }
    public vec3 qpp { get => new(q, p, p); set { q = value.x; p = value.y; p = value.z; } }
    public vec3 qpq { get => new(q, p, q); set { q = value.x; p = value.y; q = value.z; } }
    public vec3 qqs { get => new(q, q, s); set { q = value.x; q = value.y; s = value.z; } }
    public vec3 qqt { get => new(q, q, t); set { q = value.x; q = value.y; t = value.z; } }
    public vec3 qqp { get => new(q, q, p); set { q = value.x; q = value.y; p = value.z; } }
    public vec3 qqq { get => new(q, q, q); set { q = value.x; q = value.y; q = value.z; } }
    public vec4 xxxx { get => new(x, x, x, x); set { x = value.x; x = value.y; x = value.z; x = value.w; } }
    public vec4 xxxy { get => new(x, x, x, y); set { x = value.x; x = value.y; x = value.z; y = value.w; } }
    public vec4 xxxz { get => new(x, x, x, z); set { x = value.x; x = value.y; x = value.z; z = value.w; } }
    public vec4 xxxw { get => new(x, x, x, w); set { x = value.x; x = value.y; x = value.z; w = value.w; } }
    public vec4 xxyx { get => new(x, x, y, x); set { x = value.x; x = value.y; y = value.z; x = value.w; } }
    public vec4 xxyy { get => new(x, x, y, y); set { x = value.x; x = value.y; y = value.z; y = value.w; } }
    public vec4 xxyz { get => new(x, x, y, z); set { x = value.x; x = value.y; y = value.z; z = value.w; } }
    public vec4 xxyw { get => new(x, x, y, w); set { x = value.x; x = value.y; y = value.z; w = value.w; } }
    public vec4 xxzx { get => new(x, x, z, x); set { x = value.x; x = value.y; z = value.z; x = value.w; } }
    public vec4 xxzy { get => new(x, x, z, y); set { x = value.x; x = value.y; z = value.z; y = value.w; } }
    public vec4 xxzz { get => new(x, x, z, z); set { x = value.x; x = value.y; z = value.z; z = value.w; } }
    public vec4 xxzw { get => new(x, x, z, w); set { x = value.x; x = value.y; z = value.z; w = value.w; } }
    public vec4 xxwx { get => new(x, x, w, x); set { x = value.x; x = value.y; w = value.z; x = value.w; } }
    public vec4 xxwy { get => new(x, x, w, y); set { x = value.x; x = value.y; w = value.z; y = value.w; } }
    public vec4 xxwz { get => new(x, x, w, z); set { x = value.x; x = value.y; w = value.z; z = value.w; } }
    public vec4 xxww { get => new(x, x, w, w); set { x = value.x; x = value.y; w = value.z; w = value.w; } }
    public vec4 xyxx { get => new(x, y, x, x); set { x = value.x; y = value.y; x = value.z; x = value.w; } }
    public vec4 xyxy { get => new(x, y, x, y); set { x = value.x; y = value.y; x = value.z; y = value.w; } }
    public vec4 xyxz { get => new(x, y, x, z); set { x = value.x; y = value.y; x = value.z; z = value.w; } }
    public vec4 xyxw { get => new(x, y, x, w); set { x = value.x; y = value.y; x = value.z; w = value.w; } }
    public vec4 xyyx { get => new(x, y, y, x); set { x = value.x; y = value.y; y = value.z; x = value.w; } }
    public vec4 xyyy { get => new(x, y, y, y); set { x = value.x; y = value.y; y = value.z; y = value.w; } }
    public vec4 xyyz { get => new(x, y, y, z); set { x = value.x; y = value.y; y = value.z; z = value.w; } }
    public vec4 xyyw { get => new(x, y, y, w); set { x = value.x; y = value.y; y = value.z; w = value.w; } }
    public vec4 xyzx { get => new(x, y, z, x); set { x = value.x; y = value.y; z = value.z; x = value.w; } }
    public vec4 xyzy { get => new(x, y, z, y); set { x = value.x; y = value.y; z = value.z; y = value.w; } }
    public vec4 xyzz { get => new(x, y, z, z); set { x = value.x; y = value.y; z = value.z; z = value.w; } }
    public vec4 xyzw { get => new(x, y, z, w); set { x = value.x; y = value.y; z = value.z; w = value.w; } }
    public vec4 xywx { get => new(x, y, w, x); set { x = value.x; y = value.y; w = value.z; x = value.w; } }
    public vec4 xywy { get => new(x, y, w, y); set { x = value.x; y = value.y; w = value.z; y = value.w; } }
    public vec4 xywz { get => new(x, y, w, z); set { x = value.x; y = value.y; w = value.z; z = value.w; } }
    public vec4 xyww { get => new(x, y, w, w); set { x = value.x; y = value.y; w = value.z; w = value.w; } }
    public vec4 xzxx { get => new(x, z, x, x); set { x = value.x; z = value.y; x = value.z; x = value.w; } }
    public vec4 xzxy { get => new(x, z, x, y); set { x = value.x; z = value.y; x = value.z; y = value.w; } }
    public vec4 xzxz { get => new(x, z, x, z); set { x = value.x; z = value.y; x = value.z; z = value.w; } }
    public vec4 xzxw { get => new(x, z, x, w); set { x = value.x; z = value.y; x = value.z; w = value.w; } }
    public vec4 xzyx { get => new(x, z, y, x); set { x = value.x; z = value.y; y = value.z; x = value.w; } }
    public vec4 xzyy { get => new(x, z, y, y); set { x = value.x; z = value.y; y = value.z; y = value.w; } }
    public vec4 xzyz { get => new(x, z, y, z); set { x = value.x; z = value.y; y = value.z; z = value.w; } }
    public vec4 xzyw { get => new(x, z, y, w); set { x = value.x; z = value.y; y = value.z; w = value.w; } }
    public vec4 xzzx { get => new(x, z, z, x); set { x = value.x; z = value.y; z = value.z; x = value.w; } }
    public vec4 xzzy { get => new(x, z, z, y); set { x = value.x; z = value.y; z = value.z; y = value.w; } }
    public vec4 xzzz { get => new(x, z, z, z); set { x = value.x; z = value.y; z = value.z; z = value.w; } }
    public vec4 xzzw { get => new(x, z, z, w); set { x = value.x; z = value.y; z = value.z; w = value.w; } }
    public vec4 xzwx { get => new(x, z, w, x); set { x = value.x; z = value.y; w = value.z; x = value.w; } }
    public vec4 xzwy { get => new(x, z, w, y); set { x = value.x; z = value.y; w = value.z; y = value.w; } }
    public vec4 xzwz { get => new(x, z, w, z); set { x = value.x; z = value.y; w = value.z; z = value.w; } }
    public vec4 xzww { get => new(x, z, w, w); set { x = value.x; z = value.y; w = value.z; w = value.w; } }
    public vec4 xwxx { get => new(x, w, x, x); set { x = value.x; w = value.y; x = value.z; x = value.w; } }
    public vec4 xwxy { get => new(x, w, x, y); set { x = value.x; w = value.y; x = value.z; y = value.w; } }
    public vec4 xwxz { get => new(x, w, x, z); set { x = value.x; w = value.y; x = value.z; z = value.w; } }
    public vec4 xwxw { get => new(x, w, x, w); set { x = value.x; w = value.y; x = value.z; w = value.w; } }
    public vec4 xwyx { get => new(x, w, y, x); set { x = value.x; w = value.y; y = value.z; x = value.w; } }
    public vec4 xwyy { get => new(x, w, y, y); set { x = value.x; w = value.y; y = value.z; y = value.w; } }
    public vec4 xwyz { get => new(x, w, y, z); set { x = value.x; w = value.y; y = value.z; z = value.w; } }
    public vec4 xwyw { get => new(x, w, y, w); set { x = value.x; w = value.y; y = value.z; w = value.w; } }
    public vec4 xwzx { get => new(x, w, z, x); set { x = value.x; w = value.y; z = value.z; x = value.w; } }
    public vec4 xwzy { get => new(x, w, z, y); set { x = value.x; w = value.y; z = value.z; y = value.w; } }
    public vec4 xwzz { get => new(x, w, z, z); set { x = value.x; w = value.y; z = value.z; z = value.w; } }
    public vec4 xwzw { get => new(x, w, z, w); set { x = value.x; w = value.y; z = value.z; w = value.w; } }
    public vec4 xwwx { get => new(x, w, w, x); set { x = value.x; w = value.y; w = value.z; x = value.w; } }
    public vec4 xwwy { get => new(x, w, w, y); set { x = value.x; w = value.y; w = value.z; y = value.w; } }
    public vec4 xwwz { get => new(x, w, w, z); set { x = value.x; w = value.y; w = value.z; z = value.w; } }
    public vec4 xwww { get => new(x, w, w, w); set { x = value.x; w = value.y; w = value.z; w = value.w; } }
    public vec4 yxxx { get => new(y, x, x, x); set { y = value.x; x = value.y; x = value.z; x = value.w; } }
    public vec4 yxxy { get => new(y, x, x, y); set { y = value.x; x = value.y; x = value.z; y = value.w; } }
    public vec4 yxxz { get => new(y, x, x, z); set { y = value.x; x = value.y; x = value.z; z = value.w; } }
    public vec4 yxxw { get => new(y, x, x, w); set { y = value.x; x = value.y; x = value.z; w = value.w; } }
    public vec4 yxyx { get => new(y, x, y, x); set { y = value.x; x = value.y; y = value.z; x = value.w; } }
    public vec4 yxyy { get => new(y, x, y, y); set { y = value.x; x = value.y; y = value.z; y = value.w; } }
    public vec4 yxyz { get => new(y, x, y, z); set { y = value.x; x = value.y; y = value.z; z = value.w; } }
    public vec4 yxyw { get => new(y, x, y, w); set { y = value.x; x = value.y; y = value.z; w = value.w; } }
    public vec4 yxzx { get => new(y, x, z, x); set { y = value.x; x = value.y; z = value.z; x = value.w; } }
    public vec4 yxzy { get => new(y, x, z, y); set { y = value.x; x = value.y; z = value.z; y = value.w; } }
    public vec4 yxzz { get => new(y, x, z, z); set { y = value.x; x = value.y; z = value.z; z = value.w; } }
    public vec4 yxzw { get => new(y, x, z, w); set { y = value.x; x = value.y; z = value.z; w = value.w; } }
    public vec4 yxwx { get => new(y, x, w, x); set { y = value.x; x = value.y; w = value.z; x = value.w; } }
    public vec4 yxwy { get => new(y, x, w, y); set { y = value.x; x = value.y; w = value.z; y = value.w; } }
    public vec4 yxwz { get => new(y, x, w, z); set { y = value.x; x = value.y; w = value.z; z = value.w; } }
    public vec4 yxww { get => new(y, x, w, w); set { y = value.x; x = value.y; w = value.z; w = value.w; } }
    public vec4 yyxx { get => new(y, y, x, x); set { y = value.x; y = value.y; x = value.z; x = value.w; } }
    public vec4 yyxy { get => new(y, y, x, y); set { y = value.x; y = value.y; x = value.z; y = value.w; } }
    public vec4 yyxz { get => new(y, y, x, z); set { y = value.x; y = value.y; x = value.z; z = value.w; } }
    public vec4 yyxw { get => new(y, y, x, w); set { y = value.x; y = value.y; x = value.z; w = value.w; } }
    public vec4 yyyx { get => new(y, y, y, x); set { y = value.x; y = value.y; y = value.z; x = value.w; } }
    public vec4 yyyy { get => new(y, y, y, y); set { y = value.x; y = value.y; y = value.z; y = value.w; } }
    public vec4 yyyz { get => new(y, y, y, z); set { y = value.x; y = value.y; y = value.z; z = value.w; } }
    public vec4 yyyw { get => new(y, y, y, w); set { y = value.x; y = value.y; y = value.z; w = value.w; } }
    public vec4 yyzx { get => new(y, y, z, x); set { y = value.x; y = value.y; z = value.z; x = value.w; } }
    public vec4 yyzy { get => new(y, y, z, y); set { y = value.x; y = value.y; z = value.z; y = value.w; } }
    public vec4 yyzz { get => new(y, y, z, z); set { y = value.x; y = value.y; z = value.z; z = value.w; } }
    public vec4 yyzw { get => new(y, y, z, w); set { y = value.x; y = value.y; z = value.z; w = value.w; } }
    public vec4 yywx { get => new(y, y, w, x); set { y = value.x; y = value.y; w = value.z; x = value.w; } }
    public vec4 yywy { get => new(y, y, w, y); set { y = value.x; y = value.y; w = value.z; y = value.w; } }
    public vec4 yywz { get => new(y, y, w, z); set { y = value.x; y = value.y; w = value.z; z = value.w; } }
    public vec4 yyww { get => new(y, y, w, w); set { y = value.x; y = value.y; w = value.z; w = value.w; } }
    public vec4 yzxx { get => new(y, z, x, x); set { y = value.x; z = value.y; x = value.z; x = value.w; } }
    public vec4 yzxy { get => new(y, z, x, y); set { y = value.x; z = value.y; x = value.z; y = value.w; } }
    public vec4 yzxz { get => new(y, z, x, z); set { y = value.x; z = value.y; x = value.z; z = value.w; } }
    public vec4 yzxw { get => new(y, z, x, w); set { y = value.x; z = value.y; x = value.z; w = value.w; } }
    public vec4 yzyx { get => new(y, z, y, x); set { y = value.x; z = value.y; y = value.z; x = value.w; } }
    public vec4 yzyy { get => new(y, z, y, y); set { y = value.x; z = value.y; y = value.z; y = value.w; } }
    public vec4 yzyz { get => new(y, z, y, z); set { y = value.x; z = value.y; y = value.z; z = value.w; } }
    public vec4 yzyw { get => new(y, z, y, w); set { y = value.x; z = value.y; y = value.z; w = value.w; } }
    public vec4 yzzx { get => new(y, z, z, x); set { y = value.x; z = value.y; z = value.z; x = value.w; } }
    public vec4 yzzy { get => new(y, z, z, y); set { y = value.x; z = value.y; z = value.z; y = value.w; } }
    public vec4 yzzz { get => new(y, z, z, z); set { y = value.x; z = value.y; z = value.z; z = value.w; } }
    public vec4 yzzw { get => new(y, z, z, w); set { y = value.x; z = value.y; z = value.z; w = value.w; } }
    public vec4 yzwx { get => new(y, z, w, x); set { y = value.x; z = value.y; w = value.z; x = value.w; } }
    public vec4 yzwy { get => new(y, z, w, y); set { y = value.x; z = value.y; w = value.z; y = value.w; } }
    public vec4 yzwz { get => new(y, z, w, z); set { y = value.x; z = value.y; w = value.z; z = value.w; } }
    public vec4 yzww { get => new(y, z, w, w); set { y = value.x; z = value.y; w = value.z; w = value.w; } }
    public vec4 ywxx { get => new(y, w, x, x); set { y = value.x; w = value.y; x = value.z; x = value.w; } }
    public vec4 ywxy { get => new(y, w, x, y); set { y = value.x; w = value.y; x = value.z; y = value.w; } }
    public vec4 ywxz { get => new(y, w, x, z); set { y = value.x; w = value.y; x = value.z; z = value.w; } }
    public vec4 ywxw { get => new(y, w, x, w); set { y = value.x; w = value.y; x = value.z; w = value.w; } }
    public vec4 ywyx { get => new(y, w, y, x); set { y = value.x; w = value.y; y = value.z; x = value.w; } }
    public vec4 ywyy { get => new(y, w, y, y); set { y = value.x; w = value.y; y = value.z; y = value.w; } }
    public vec4 ywyz { get => new(y, w, y, z); set { y = value.x; w = value.y; y = value.z; z = value.w; } }
    public vec4 ywyw { get => new(y, w, y, w); set { y = value.x; w = value.y; y = value.z; w = value.w; } }
    public vec4 ywzx { get => new(y, w, z, x); set { y = value.x; w = value.y; z = value.z; x = value.w; } }
    public vec4 ywzy { get => new(y, w, z, y); set { y = value.x; w = value.y; z = value.z; y = value.w; } }
    public vec4 ywzz { get => new(y, w, z, z); set { y = value.x; w = value.y; z = value.z; z = value.w; } }
    public vec4 ywzw { get => new(y, w, z, w); set { y = value.x; w = value.y; z = value.z; w = value.w; } }
    public vec4 ywwx { get => new(y, w, w, x); set { y = value.x; w = value.y; w = value.z; x = value.w; } }
    public vec4 ywwy { get => new(y, w, w, y); set { y = value.x; w = value.y; w = value.z; y = value.w; } }
    public vec4 ywwz { get => new(y, w, w, z); set { y = value.x; w = value.y; w = value.z; z = value.w; } }
    public vec4 ywww { get => new(y, w, w, w); set { y = value.x; w = value.y; w = value.z; w = value.w; } }
    public vec4 zxxx { get => new(z, x, x, x); set { z = value.x; x = value.y; x = value.z; x = value.w; } }
    public vec4 zxxy { get => new(z, x, x, y); set { z = value.x; x = value.y; x = value.z; y = value.w; } }
    public vec4 zxxz { get => new(z, x, x, z); set { z = value.x; x = value.y; x = value.z; z = value.w; } }
    public vec4 zxxw { get => new(z, x, x, w); set { z = value.x; x = value.y; x = value.z; w = value.w; } }
    public vec4 zxyx { get => new(z, x, y, x); set { z = value.x; x = value.y; y = value.z; x = value.w; } }
    public vec4 zxyy { get => new(z, x, y, y); set { z = value.x; x = value.y; y = value.z; y = value.w; } }
    public vec4 zxyz { get => new(z, x, y, z); set { z = value.x; x = value.y; y = value.z; z = value.w; } }
    public vec4 zxyw { get => new(z, x, y, w); set { z = value.x; x = value.y; y = value.z; w = value.w; } }
    public vec4 zxzx { get => new(z, x, z, x); set { z = value.x; x = value.y; z = value.z; x = value.w; } }
    public vec4 zxzy { get => new(z, x, z, y); set { z = value.x; x = value.y; z = value.z; y = value.w; } }
    public vec4 zxzz { get => new(z, x, z, z); set { z = value.x; x = value.y; z = value.z; z = value.w; } }
    public vec4 zxzw { get => new(z, x, z, w); set { z = value.x; x = value.y; z = value.z; w = value.w; } }
    public vec4 zxwx { get => new(z, x, w, x); set { z = value.x; x = value.y; w = value.z; x = value.w; } }
    public vec4 zxwy { get => new(z, x, w, y); set { z = value.x; x = value.y; w = value.z; y = value.w; } }
    public vec4 zxwz { get => new(z, x, w, z); set { z = value.x; x = value.y; w = value.z; z = value.w; } }
    public vec4 zxww { get => new(z, x, w, w); set { z = value.x; x = value.y; w = value.z; w = value.w; } }
    public vec4 zyxx { get => new(z, y, x, x); set { z = value.x; y = value.y; x = value.z; x = value.w; } }
    public vec4 zyxy { get => new(z, y, x, y); set { z = value.x; y = value.y; x = value.z; y = value.w; } }
    public vec4 zyxz { get => new(z, y, x, z); set { z = value.x; y = value.y; x = value.z; z = value.w; } }
    public vec4 zyxw { get => new(z, y, x, w); set { z = value.x; y = value.y; x = value.z; w = value.w; } }
    public vec4 zyyx { get => new(z, y, y, x); set { z = value.x; y = value.y; y = value.z; x = value.w; } }
    public vec4 zyyy { get => new(z, y, y, y); set { z = value.x; y = value.y; y = value.z; y = value.w; } }
    public vec4 zyyz { get => new(z, y, y, z); set { z = value.x; y = value.y; y = value.z; z = value.w; } }
    public vec4 zyyw { get => new(z, y, y, w); set { z = value.x; y = value.y; y = value.z; w = value.w; } }
    public vec4 zyzx { get => new(z, y, z, x); set { z = value.x; y = value.y; z = value.z; x = value.w; } }
    public vec4 zyzy { get => new(z, y, z, y); set { z = value.x; y = value.y; z = value.z; y = value.w; } }
    public vec4 zyzz { get => new(z, y, z, z); set { z = value.x; y = value.y; z = value.z; z = value.w; } }
    public vec4 zyzw { get => new(z, y, z, w); set { z = value.x; y = value.y; z = value.z; w = value.w; } }
    public vec4 zywx { get => new(z, y, w, x); set { z = value.x; y = value.y; w = value.z; x = value.w; } }
    public vec4 zywy { get => new(z, y, w, y); set { z = value.x; y = value.y; w = value.z; y = value.w; } }
    public vec4 zywz { get => new(z, y, w, z); set { z = value.x; y = value.y; w = value.z; z = value.w; } }
    public vec4 zyww { get => new(z, y, w, w); set { z = value.x; y = value.y; w = value.z; w = value.w; } }
    public vec4 zzxx { get => new(z, z, x, x); set { z = value.x; z = value.y; x = value.z; x = value.w; } }
    public vec4 zzxy { get => new(z, z, x, y); set { z = value.x; z = value.y; x = value.z; y = value.w; } }
    public vec4 zzxz { get => new(z, z, x, z); set { z = value.x; z = value.y; x = value.z; z = value.w; } }
    public vec4 zzxw { get => new(z, z, x, w); set { z = value.x; z = value.y; x = value.z; w = value.w; } }
    public vec4 zzyx { get => new(z, z, y, x); set { z = value.x; z = value.y; y = value.z; x = value.w; } }
    public vec4 zzyy { get => new(z, z, y, y); set { z = value.x; z = value.y; y = value.z; y = value.w; } }
    public vec4 zzyz { get => new(z, z, y, z); set { z = value.x; z = value.y; y = value.z; z = value.w; } }
    public vec4 zzyw { get => new(z, z, y, w); set { z = value.x; z = value.y; y = value.z; w = value.w; } }
    public vec4 zzzx { get => new(z, z, z, x); set { z = value.x; z = value.y; z = value.z; x = value.w; } }
    public vec4 zzzy { get => new(z, z, z, y); set { z = value.x; z = value.y; z = value.z; y = value.w; } }
    public vec4 zzzz { get => new(z, z, z, z); set { z = value.x; z = value.y; z = value.z; z = value.w; } }
    public vec4 zzzw { get => new(z, z, z, w); set { z = value.x; z = value.y; z = value.z; w = value.w; } }
    public vec4 zzwx { get => new(z, z, w, x); set { z = value.x; z = value.y; w = value.z; x = value.w; } }
    public vec4 zzwy { get => new(z, z, w, y); set { z = value.x; z = value.y; w = value.z; y = value.w; } }
    public vec4 zzwz { get => new(z, z, w, z); set { z = value.x; z = value.y; w = value.z; z = value.w; } }
    public vec4 zzww { get => new(z, z, w, w); set { z = value.x; z = value.y; w = value.z; w = value.w; } }
    public vec4 zwxx { get => new(z, w, x, x); set { z = value.x; w = value.y; x = value.z; x = value.w; } }
    public vec4 zwxy { get => new(z, w, x, y); set { z = value.x; w = value.y; x = value.z; y = value.w; } }
    public vec4 zwxz { get => new(z, w, x, z); set { z = value.x; w = value.y; x = value.z; z = value.w; } }
    public vec4 zwxw { get => new(z, w, x, w); set { z = value.x; w = value.y; x = value.z; w = value.w; } }
    public vec4 zwyx { get => new(z, w, y, x); set { z = value.x; w = value.y; y = value.z; x = value.w; } }
    public vec4 zwyy { get => new(z, w, y, y); set { z = value.x; w = value.y; y = value.z; y = value.w; } }
    public vec4 zwyz { get => new(z, w, y, z); set { z = value.x; w = value.y; y = value.z; z = value.w; } }
    public vec4 zwyw { get => new(z, w, y, w); set { z = value.x; w = value.y; y = value.z; w = value.w; } }
    public vec4 zwzx { get => new(z, w, z, x); set { z = value.x; w = value.y; z = value.z; x = value.w; } }
    public vec4 zwzy { get => new(z, w, z, y); set { z = value.x; w = value.y; z = value.z; y = value.w; } }
    public vec4 zwzz { get => new(z, w, z, z); set { z = value.x; w = value.y; z = value.z; z = value.w; } }
    public vec4 zwzw { get => new(z, w, z, w); set { z = value.x; w = value.y; z = value.z; w = value.w; } }
    public vec4 zwwx { get => new(z, w, w, x); set { z = value.x; w = value.y; w = value.z; x = value.w; } }
    public vec4 zwwy { get => new(z, w, w, y); set { z = value.x; w = value.y; w = value.z; y = value.w; } }
    public vec4 zwwz { get => new(z, w, w, z); set { z = value.x; w = value.y; w = value.z; z = value.w; } }
    public vec4 zwww { get => new(z, w, w, w); set { z = value.x; w = value.y; w = value.z; w = value.w; } }
    public vec4 wxxx { get => new(w, x, x, x); set { w = value.x; x = value.y; x = value.z; x = value.w; } }
    public vec4 wxxy { get => new(w, x, x, y); set { w = value.x; x = value.y; x = value.z; y = value.w; } }
    public vec4 wxxz { get => new(w, x, x, z); set { w = value.x; x = value.y; x = value.z; z = value.w; } }
    public vec4 wxxw { get => new(w, x, x, w); set { w = value.x; x = value.y; x = value.z; w = value.w; } }
    public vec4 wxyx { get => new(w, x, y, x); set { w = value.x; x = value.y; y = value.z; x = value.w; } }
    public vec4 wxyy { get => new(w, x, y, y); set { w = value.x; x = value.y; y = value.z; y = value.w; } }
    public vec4 wxyz { get => new(w, x, y, z); set { w = value.x; x = value.y; y = value.z; z = value.w; } }
    public vec4 wxyw { get => new(w, x, y, w); set { w = value.x; x = value.y; y = value.z; w = value.w; } }
    public vec4 wxzx { get => new(w, x, z, x); set { w = value.x; x = value.y; z = value.z; x = value.w; } }
    public vec4 wxzy { get => new(w, x, z, y); set { w = value.x; x = value.y; z = value.z; y = value.w; } }
    public vec4 wxzz { get => new(w, x, z, z); set { w = value.x; x = value.y; z = value.z; z = value.w; } }
    public vec4 wxzw { get => new(w, x, z, w); set { w = value.x; x = value.y; z = value.z; w = value.w; } }
    public vec4 wxwx { get => new(w, x, w, x); set { w = value.x; x = value.y; w = value.z; x = value.w; } }
    public vec4 wxwy { get => new(w, x, w, y); set { w = value.x; x = value.y; w = value.z; y = value.w; } }
    public vec4 wxwz { get => new(w, x, w, z); set { w = value.x; x = value.y; w = value.z; z = value.w; } }
    public vec4 wxww { get => new(w, x, w, w); set { w = value.x; x = value.y; w = value.z; w = value.w; } }
    public vec4 wyxx { get => new(w, y, x, x); set { w = value.x; y = value.y; x = value.z; x = value.w; } }
    public vec4 wyxy { get => new(w, y, x, y); set { w = value.x; y = value.y; x = value.z; y = value.w; } }
    public vec4 wyxz { get => new(w, y, x, z); set { w = value.x; y = value.y; x = value.z; z = value.w; } }
    public vec4 wyxw { get => new(w, y, x, w); set { w = value.x; y = value.y; x = value.z; w = value.w; } }
    public vec4 wyyx { get => new(w, y, y, x); set { w = value.x; y = value.y; y = value.z; x = value.w; } }
    public vec4 wyyy { get => new(w, y, y, y); set { w = value.x; y = value.y; y = value.z; y = value.w; } }
    public vec4 wyyz { get => new(w, y, y, z); set { w = value.x; y = value.y; y = value.z; z = value.w; } }
    public vec4 wyyw { get => new(w, y, y, w); set { w = value.x; y = value.y; y = value.z; w = value.w; } }
    public vec4 wyzx { get => new(w, y, z, x); set { w = value.x; y = value.y; z = value.z; x = value.w; } }
    public vec4 wyzy { get => new(w, y, z, y); set { w = value.x; y = value.y; z = value.z; y = value.w; } }
    public vec4 wyzz { get => new(w, y, z, z); set { w = value.x; y = value.y; z = value.z; z = value.w; } }
    public vec4 wyzw { get => new(w, y, z, w); set { w = value.x; y = value.y; z = value.z; w = value.w; } }
    public vec4 wywx { get => new(w, y, w, x); set { w = value.x; y = value.y; w = value.z; x = value.w; } }
    public vec4 wywy { get => new(w, y, w, y); set { w = value.x; y = value.y; w = value.z; y = value.w; } }
    public vec4 wywz { get => new(w, y, w, z); set { w = value.x; y = value.y; w = value.z; z = value.w; } }
    public vec4 wyww { get => new(w, y, w, w); set { w = value.x; y = value.y; w = value.z; w = value.w; } }
    public vec4 wzxx { get => new(w, z, x, x); set { w = value.x; z = value.y; x = value.z; x = value.w; } }
    public vec4 wzxy { get => new(w, z, x, y); set { w = value.x; z = value.y; x = value.z; y = value.w; } }
    public vec4 wzxz { get => new(w, z, x, z); set { w = value.x; z = value.y; x = value.z; z = value.w; } }
    public vec4 wzxw { get => new(w, z, x, w); set { w = value.x; z = value.y; x = value.z; w = value.w; } }
    public vec4 wzyx { get => new(w, z, y, x); set { w = value.x; z = value.y; y = value.z; x = value.w; } }
    public vec4 wzyy { get => new(w, z, y, y); set { w = value.x; z = value.y; y = value.z; y = value.w; } }
    public vec4 wzyz { get => new(w, z, y, z); set { w = value.x; z = value.y; y = value.z; z = value.w; } }
    public vec4 wzyw { get => new(w, z, y, w); set { w = value.x; z = value.y; y = value.z; w = value.w; } }
    public vec4 wzzx { get => new(w, z, z, x); set { w = value.x; z = value.y; z = value.z; x = value.w; } }
    public vec4 wzzy { get => new(w, z, z, y); set { w = value.x; z = value.y; z = value.z; y = value.w; } }
    public vec4 wzzz { get => new(w, z, z, z); set { w = value.x; z = value.y; z = value.z; z = value.w; } }
    public vec4 wzzw { get => new(w, z, z, w); set { w = value.x; z = value.y; z = value.z; w = value.w; } }
    public vec4 wzwx { get => new(w, z, w, x); set { w = value.x; z = value.y; w = value.z; x = value.w; } }
    public vec4 wzwy { get => new(w, z, w, y); set { w = value.x; z = value.y; w = value.z; y = value.w; } }
    public vec4 wzwz { get => new(w, z, w, z); set { w = value.x; z = value.y; w = value.z; z = value.w; } }
    public vec4 wzww { get => new(w, z, w, w); set { w = value.x; z = value.y; w = value.z; w = value.w; } }
    public vec4 wwxx { get => new(w, w, x, x); set { w = value.x; w = value.y; x = value.z; x = value.w; } }
    public vec4 wwxy { get => new(w, w, x, y); set { w = value.x; w = value.y; x = value.z; y = value.w; } }
    public vec4 wwxz { get => new(w, w, x, z); set { w = value.x; w = value.y; x = value.z; z = value.w; } }
    public vec4 wwxw { get => new(w, w, x, w); set { w = value.x; w = value.y; x = value.z; w = value.w; } }
    public vec4 wwyx { get => new(w, w, y, x); set { w = value.x; w = value.y; y = value.z; x = value.w; } }
    public vec4 wwyy { get => new(w, w, y, y); set { w = value.x; w = value.y; y = value.z; y = value.w; } }
    public vec4 wwyz { get => new(w, w, y, z); set { w = value.x; w = value.y; y = value.z; z = value.w; } }
    public vec4 wwyw { get => new(w, w, y, w); set { w = value.x; w = value.y; y = value.z; w = value.w; } }
    public vec4 wwzx { get => new(w, w, z, x); set { w = value.x; w = value.y; z = value.z; x = value.w; } }
    public vec4 wwzy { get => new(w, w, z, y); set { w = value.x; w = value.y; z = value.z; y = value.w; } }
    public vec4 wwzz { get => new(w, w, z, z); set { w = value.x; w = value.y; z = value.z; z = value.w; } }
    public vec4 wwzw { get => new(w, w, z, w); set { w = value.x; w = value.y; z = value.z; w = value.w; } }
    public vec4 wwwx { get => new(w, w, w, x); set { w = value.x; w = value.y; w = value.z; x = value.w; } }
    public vec4 wwwy { get => new(w, w, w, y); set { w = value.x; w = value.y; w = value.z; y = value.w; } }
    public vec4 wwwz { get => new(w, w, w, z); set { w = value.x; w = value.y; w = value.z; z = value.w; } }
    public vec4 wwww { get => new(w, w, w, w); set { w = value.x; w = value.y; w = value.z; w = value.w; } }
    public vec4 rrrr { get => new(r, r, r, r); set { r = value.x; r = value.y; r = value.z; r = value.w; } }
    public vec4 rrrg { get => new(r, r, r, g); set { r = value.x; r = value.y; r = value.z; g = value.w; } }
    public vec4 rrrb { get => new(r, r, r, b); set { r = value.x; r = value.y; r = value.z; b = value.w; } }
    public vec4 rrra { get => new(r, r, r, a); set { r = value.x; r = value.y; r = value.z; a = value.w; } }
    public vec4 rrgr { get => new(r, r, g, r); set { r = value.x; r = value.y; g = value.z; r = value.w; } }
    public vec4 rrgg { get => new(r, r, g, g); set { r = value.x; r = value.y; g = value.z; g = value.w; } }
    public vec4 rrgb { get => new(r, r, g, b); set { r = value.x; r = value.y; g = value.z; b = value.w; } }
    public vec4 rrga { get => new(r, r, g, a); set { r = value.x; r = value.y; g = value.z; a = value.w; } }
    public vec4 rrbr { get => new(r, r, b, r); set { r = value.x; r = value.y; b = value.z; r = value.w; } }
    public vec4 rrbg { get => new(r, r, b, g); set { r = value.x; r = value.y; b = value.z; g = value.w; } }
    public vec4 rrbb { get => new(r, r, b, b); set { r = value.x; r = value.y; b = value.z; b = value.w; } }
    public vec4 rrba { get => new(r, r, b, a); set { r = value.x; r = value.y; b = value.z; a = value.w; } }
    public vec4 rrar { get => new(r, r, a, r); set { r = value.x; r = value.y; a = value.z; r = value.w; } }
    public vec4 rrag { get => new(r, r, a, g); set { r = value.x; r = value.y; a = value.z; g = value.w; } }
    public vec4 rrab { get => new(r, r, a, b); set { r = value.x; r = value.y; a = value.z; b = value.w; } }
    public vec4 rraa { get => new(r, r, a, a); set { r = value.x; r = value.y; a = value.z; a = value.w; } }
    public vec4 rgrr { get => new(r, g, r, r); set { r = value.x; g = value.y; r = value.z; r = value.w; } }
    public vec4 rgrg { get => new(r, g, r, g); set { r = value.x; g = value.y; r = value.z; g = value.w; } }
    public vec4 rgrb { get => new(r, g, r, b); set { r = value.x; g = value.y; r = value.z; b = value.w; } }
    public vec4 rgra { get => new(r, g, r, a); set { r = value.x; g = value.y; r = value.z; a = value.w; } }
    public vec4 rggr { get => new(r, g, g, r); set { r = value.x; g = value.y; g = value.z; r = value.w; } }
    public vec4 rggg { get => new(r, g, g, g); set { r = value.x; g = value.y; g = value.z; g = value.w; } }
    public vec4 rggb { get => new(r, g, g, b); set { r = value.x; g = value.y; g = value.z; b = value.w; } }
    public vec4 rgga { get => new(r, g, g, a); set { r = value.x; g = value.y; g = value.z; a = value.w; } }
    public vec4 rgbr { get => new(r, g, b, r); set { r = value.x; g = value.y; b = value.z; r = value.w; } }
    public vec4 rgbg { get => new(r, g, b, g); set { r = value.x; g = value.y; b = value.z; g = value.w; } }
    public vec4 rgbb { get => new(r, g, b, b); set { r = value.x; g = value.y; b = value.z; b = value.w; } }
    public vec4 rgba { get => new(r, g, b, a); set { r = value.x; g = value.y; b = value.z; a = value.w; } }
    public vec4 rgar { get => new(r, g, a, r); set { r = value.x; g = value.y; a = value.z; r = value.w; } }
    public vec4 rgag { get => new(r, g, a, g); set { r = value.x; g = value.y; a = value.z; g = value.w; } }
    public vec4 rgab { get => new(r, g, a, b); set { r = value.x; g = value.y; a = value.z; b = value.w; } }
    public vec4 rgaa { get => new(r, g, a, a); set { r = value.x; g = value.y; a = value.z; a = value.w; } }
    public vec4 rbrr { get => new(r, b, r, r); set { r = value.x; b = value.y; r = value.z; r = value.w; } }
    public vec4 rbrg { get => new(r, b, r, g); set { r = value.x; b = value.y; r = value.z; g = value.w; } }
    public vec4 rbrb { get => new(r, b, r, b); set { r = value.x; b = value.y; r = value.z; b = value.w; } }
    public vec4 rbra { get => new(r, b, r, a); set { r = value.x; b = value.y; r = value.z; a = value.w; } }
    public vec4 rbgr { get => new(r, b, g, r); set { r = value.x; b = value.y; g = value.z; r = value.w; } }
    public vec4 rbgg { get => new(r, b, g, g); set { r = value.x; b = value.y; g = value.z; g = value.w; } }
    public vec4 rbgb { get => new(r, b, g, b); set { r = value.x; b = value.y; g = value.z; b = value.w; } }
    public vec4 rbga { get => new(r, b, g, a); set { r = value.x; b = value.y; g = value.z; a = value.w; } }
    public vec4 rbbr { get => new(r, b, b, r); set { r = value.x; b = value.y; b = value.z; r = value.w; } }
    public vec4 rbbg { get => new(r, b, b, g); set { r = value.x; b = value.y; b = value.z; g = value.w; } }
    public vec4 rbbb { get => new(r, b, b, b); set { r = value.x; b = value.y; b = value.z; b = value.w; } }
    public vec4 rbba { get => new(r, b, b, a); set { r = value.x; b = value.y; b = value.z; a = value.w; } }
    public vec4 rbar { get => new(r, b, a, r); set { r = value.x; b = value.y; a = value.z; r = value.w; } }
    public vec4 rbag { get => new(r, b, a, g); set { r = value.x; b = value.y; a = value.z; g = value.w; } }
    public vec4 rbab { get => new(r, b, a, b); set { r = value.x; b = value.y; a = value.z; b = value.w; } }
    public vec4 rbaa { get => new(r, b, a, a); set { r = value.x; b = value.y; a = value.z; a = value.w; } }
    public vec4 rarr { get => new(r, a, r, r); set { r = value.x; a = value.y; r = value.z; r = value.w; } }
    public vec4 rarg { get => new(r, a, r, g); set { r = value.x; a = value.y; r = value.z; g = value.w; } }
    public vec4 rarb { get => new(r, a, r, b); set { r = value.x; a = value.y; r = value.z; b = value.w; } }
    public vec4 rara { get => new(r, a, r, a); set { r = value.x; a = value.y; r = value.z; a = value.w; } }
    public vec4 ragr { get => new(r, a, g, r); set { r = value.x; a = value.y; g = value.z; r = value.w; } }
    public vec4 ragg { get => new(r, a, g, g); set { r = value.x; a = value.y; g = value.z; g = value.w; } }
    public vec4 ragb { get => new(r, a, g, b); set { r = value.x; a = value.y; g = value.z; b = value.w; } }
    public vec4 raga { get => new(r, a, g, a); set { r = value.x; a = value.y; g = value.z; a = value.w; } }
    public vec4 rabr { get => new(r, a, b, r); set { r = value.x; a = value.y; b = value.z; r = value.w; } }
    public vec4 rabg { get => new(r, a, b, g); set { r = value.x; a = value.y; b = value.z; g = value.w; } }
    public vec4 rabb { get => new(r, a, b, b); set { r = value.x; a = value.y; b = value.z; b = value.w; } }
    public vec4 raba { get => new(r, a, b, a); set { r = value.x; a = value.y; b = value.z; a = value.w; } }
    public vec4 raar { get => new(r, a, a, r); set { r = value.x; a = value.y; a = value.z; r = value.w; } }
    public vec4 raag { get => new(r, a, a, g); set { r = value.x; a = value.y; a = value.z; g = value.w; } }
    public vec4 raab { get => new(r, a, a, b); set { r = value.x; a = value.y; a = value.z; b = value.w; } }
    public vec4 raaa { get => new(r, a, a, a); set { r = value.x; a = value.y; a = value.z; a = value.w; } }
    public vec4 grrr { get => new(g, r, r, r); set { g = value.x; r = value.y; r = value.z; r = value.w; } }
    public vec4 grrg { get => new(g, r, r, g); set { g = value.x; r = value.y; r = value.z; g = value.w; } }
    public vec4 grrb { get => new(g, r, r, b); set { g = value.x; r = value.y; r = value.z; b = value.w; } }
    public vec4 grra { get => new(g, r, r, a); set { g = value.x; r = value.y; r = value.z; a = value.w; } }
    public vec4 grgr { get => new(g, r, g, r); set { g = value.x; r = value.y; g = value.z; r = value.w; } }
    public vec4 grgg { get => new(g, r, g, g); set { g = value.x; r = value.y; g = value.z; g = value.w; } }
    public vec4 grgb { get => new(g, r, g, b); set { g = value.x; r = value.y; g = value.z; b = value.w; } }
    public vec4 grga { get => new(g, r, g, a); set { g = value.x; r = value.y; g = value.z; a = value.w; } }
    public vec4 grbr { get => new(g, r, b, r); set { g = value.x; r = value.y; b = value.z; r = value.w; } }
    public vec4 grbg { get => new(g, r, b, g); set { g = value.x; r = value.y; b = value.z; g = value.w; } }
    public vec4 grbb { get => new(g, r, b, b); set { g = value.x; r = value.y; b = value.z; b = value.w; } }
    public vec4 grba { get => new(g, r, b, a); set { g = value.x; r = value.y; b = value.z; a = value.w; } }
    public vec4 grar { get => new(g, r, a, r); set { g = value.x; r = value.y; a = value.z; r = value.w; } }
    public vec4 grag { get => new(g, r, a, g); set { g = value.x; r = value.y; a = value.z; g = value.w; } }
    public vec4 grab { get => new(g, r, a, b); set { g = value.x; r = value.y; a = value.z; b = value.w; } }
    public vec4 graa { get => new(g, r, a, a); set { g = value.x; r = value.y; a = value.z; a = value.w; } }
    public vec4 ggrr { get => new(g, g, r, r); set { g = value.x; g = value.y; r = value.z; r = value.w; } }
    public vec4 ggrg { get => new(g, g, r, g); set { g = value.x; g = value.y; r = value.z; g = value.w; } }
    public vec4 ggrb { get => new(g, g, r, b); set { g = value.x; g = value.y; r = value.z; b = value.w; } }
    public vec4 ggra { get => new(g, g, r, a); set { g = value.x; g = value.y; r = value.z; a = value.w; } }
    public vec4 gggr { get => new(g, g, g, r); set { g = value.x; g = value.y; g = value.z; r = value.w; } }
    public vec4 gggg { get => new(g, g, g, g); set { g = value.x; g = value.y; g = value.z; g = value.w; } }
    public vec4 gggb { get => new(g, g, g, b); set { g = value.x; g = value.y; g = value.z; b = value.w; } }
    public vec4 ggga { get => new(g, g, g, a); set { g = value.x; g = value.y; g = value.z; a = value.w; } }
    public vec4 ggbr { get => new(g, g, b, r); set { g = value.x; g = value.y; b = value.z; r = value.w; } }
    public vec4 ggbg { get => new(g, g, b, g); set { g = value.x; g = value.y; b = value.z; g = value.w; } }
    public vec4 ggbb { get => new(g, g, b, b); set { g = value.x; g = value.y; b = value.z; b = value.w; } }
    public vec4 ggba { get => new(g, g, b, a); set { g = value.x; g = value.y; b = value.z; a = value.w; } }
    public vec4 ggar { get => new(g, g, a, r); set { g = value.x; g = value.y; a = value.z; r = value.w; } }
    public vec4 ggag { get => new(g, g, a, g); set { g = value.x; g = value.y; a = value.z; g = value.w; } }
    public vec4 ggab { get => new(g, g, a, b); set { g = value.x; g = value.y; a = value.z; b = value.w; } }
    public vec4 ggaa { get => new(g, g, a, a); set { g = value.x; g = value.y; a = value.z; a = value.w; } }
    public vec4 gbrr { get => new(g, b, r, r); set { g = value.x; b = value.y; r = value.z; r = value.w; } }
    public vec4 gbrg { get => new(g, b, r, g); set { g = value.x; b = value.y; r = value.z; g = value.w; } }
    public vec4 gbrb { get => new(g, b, r, b); set { g = value.x; b = value.y; r = value.z; b = value.w; } }
    public vec4 gbra { get => new(g, b, r, a); set { g = value.x; b = value.y; r = value.z; a = value.w; } }
    public vec4 gbgr { get => new(g, b, g, r); set { g = value.x; b = value.y; g = value.z; r = value.w; } }
    public vec4 gbgg { get => new(g, b, g, g); set { g = value.x; b = value.y; g = value.z; g = value.w; } }
    public vec4 gbgb { get => new(g, b, g, b); set { g = value.x; b = value.y; g = value.z; b = value.w; } }
    public vec4 gbga { get => new(g, b, g, a); set { g = value.x; b = value.y; g = value.z; a = value.w; } }
    public vec4 gbbr { get => new(g, b, b, r); set { g = value.x; b = value.y; b = value.z; r = value.w; } }
    public vec4 gbbg { get => new(g, b, b, g); set { g = value.x; b = value.y; b = value.z; g = value.w; } }
    public vec4 gbbb { get => new(g, b, b, b); set { g = value.x; b = value.y; b = value.z; b = value.w; } }
    public vec4 gbba { get => new(g, b, b, a); set { g = value.x; b = value.y; b = value.z; a = value.w; } }
    public vec4 gbar { get => new(g, b, a, r); set { g = value.x; b = value.y; a = value.z; r = value.w; } }
    public vec4 gbag { get => new(g, b, a, g); set { g = value.x; b = value.y; a = value.z; g = value.w; } }
    public vec4 gbab { get => new(g, b, a, b); set { g = value.x; b = value.y; a = value.z; b = value.w; } }
    public vec4 gbaa { get => new(g, b, a, a); set { g = value.x; b = value.y; a = value.z; a = value.w; } }
    public vec4 garr { get => new(g, a, r, r); set { g = value.x; a = value.y; r = value.z; r = value.w; } }
    public vec4 garg { get => new(g, a, r, g); set { g = value.x; a = value.y; r = value.z; g = value.w; } }
    public vec4 garb { get => new(g, a, r, b); set { g = value.x; a = value.y; r = value.z; b = value.w; } }
    public vec4 gara { get => new(g, a, r, a); set { g = value.x; a = value.y; r = value.z; a = value.w; } }
    public vec4 gagr { get => new(g, a, g, r); set { g = value.x; a = value.y; g = value.z; r = value.w; } }
    public vec4 gagg { get => new(g, a, g, g); set { g = value.x; a = value.y; g = value.z; g = value.w; } }
    public vec4 gagb { get => new(g, a, g, b); set { g = value.x; a = value.y; g = value.z; b = value.w; } }
    public vec4 gaga { get => new(g, a, g, a); set { g = value.x; a = value.y; g = value.z; a = value.w; } }
    public vec4 gabr { get => new(g, a, b, r); set { g = value.x; a = value.y; b = value.z; r = value.w; } }
    public vec4 gabg { get => new(g, a, b, g); set { g = value.x; a = value.y; b = value.z; g = value.w; } }
    public vec4 gabb { get => new(g, a, b, b); set { g = value.x; a = value.y; b = value.z; b = value.w; } }
    public vec4 gaba { get => new(g, a, b, a); set { g = value.x; a = value.y; b = value.z; a = value.w; } }
    public vec4 gaar { get => new(g, a, a, r); set { g = value.x; a = value.y; a = value.z; r = value.w; } }
    public vec4 gaag { get => new(g, a, a, g); set { g = value.x; a = value.y; a = value.z; g = value.w; } }
    public vec4 gaab { get => new(g, a, a, b); set { g = value.x; a = value.y; a = value.z; b = value.w; } }
    public vec4 gaaa { get => new(g, a, a, a); set { g = value.x; a = value.y; a = value.z; a = value.w; } }
    public vec4 brrr { get => new(b, r, r, r); set { b = value.x; r = value.y; r = value.z; r = value.w; } }
    public vec4 brrg { get => new(b, r, r, g); set { b = value.x; r = value.y; r = value.z; g = value.w; } }
    public vec4 brrb { get => new(b, r, r, b); set { b = value.x; r = value.y; r = value.z; b = value.w; } }
    public vec4 brra { get => new(b, r, r, a); set { b = value.x; r = value.y; r = value.z; a = value.w; } }
    public vec4 brgr { get => new(b, r, g, r); set { b = value.x; r = value.y; g = value.z; r = value.w; } }
    public vec4 brgg { get => new(b, r, g, g); set { b = value.x; r = value.y; g = value.z; g = value.w; } }
    public vec4 brgb { get => new(b, r, g, b); set { b = value.x; r = value.y; g = value.z; b = value.w; } }
    public vec4 brga { get => new(b, r, g, a); set { b = value.x; r = value.y; g = value.z; a = value.w; } }
    public vec4 brbr { get => new(b, r, b, r); set { b = value.x; r = value.y; b = value.z; r = value.w; } }
    public vec4 brbg { get => new(b, r, b, g); set { b = value.x; r = value.y; b = value.z; g = value.w; } }
    public vec4 brbb { get => new(b, r, b, b); set { b = value.x; r = value.y; b = value.z; b = value.w; } }
    public vec4 brba { get => new(b, r, b, a); set { b = value.x; r = value.y; b = value.z; a = value.w; } }
    public vec4 brar { get => new(b, r, a, r); set { b = value.x; r = value.y; a = value.z; r = value.w; } }
    public vec4 brag { get => new(b, r, a, g); set { b = value.x; r = value.y; a = value.z; g = value.w; } }
    public vec4 brab { get => new(b, r, a, b); set { b = value.x; r = value.y; a = value.z; b = value.w; } }
    public vec4 braa { get => new(b, r, a, a); set { b = value.x; r = value.y; a = value.z; a = value.w; } }
    public vec4 bgrr { get => new(b, g, r, r); set { b = value.x; g = value.y; r = value.z; r = value.w; } }
    public vec4 bgrg { get => new(b, g, r, g); set { b = value.x; g = value.y; r = value.z; g = value.w; } }
    public vec4 bgrb { get => new(b, g, r, b); set { b = value.x; g = value.y; r = value.z; b = value.w; } }
    public vec4 bgra { get => new(b, g, r, a); set { b = value.x; g = value.y; r = value.z; a = value.w; } }
    public vec4 bggr { get => new(b, g, g, r); set { b = value.x; g = value.y; g = value.z; r = value.w; } }
    public vec4 bggg { get => new(b, g, g, g); set { b = value.x; g = value.y; g = value.z; g = value.w; } }
    public vec4 bggb { get => new(b, g, g, b); set { b = value.x; g = value.y; g = value.z; b = value.w; } }
    public vec4 bgga { get => new(b, g, g, a); set { b = value.x; g = value.y; g = value.z; a = value.w; } }
    public vec4 bgbr { get => new(b, g, b, r); set { b = value.x; g = value.y; b = value.z; r = value.w; } }
    public vec4 bgbg { get => new(b, g, b, g); set { b = value.x; g = value.y; b = value.z; g = value.w; } }
    public vec4 bgbb { get => new(b, g, b, b); set { b = value.x; g = value.y; b = value.z; b = value.w; } }
    public vec4 bgba { get => new(b, g, b, a); set { b = value.x; g = value.y; b = value.z; a = value.w; } }
    public vec4 bgar { get => new(b, g, a, r); set { b = value.x; g = value.y; a = value.z; r = value.w; } }
    public vec4 bgag { get => new(b, g, a, g); set { b = value.x; g = value.y; a = value.z; g = value.w; } }
    public vec4 bgab { get => new(b, g, a, b); set { b = value.x; g = value.y; a = value.z; b = value.w; } }
    public vec4 bgaa { get => new(b, g, a, a); set { b = value.x; g = value.y; a = value.z; a = value.w; } }
    public vec4 bbrr { get => new(b, b, r, r); set { b = value.x; b = value.y; r = value.z; r = value.w; } }
    public vec4 bbrg { get => new(b, b, r, g); set { b = value.x; b = value.y; r = value.z; g = value.w; } }
    public vec4 bbrb { get => new(b, b, r, b); set { b = value.x; b = value.y; r = value.z; b = value.w; } }
    public vec4 bbra { get => new(b, b, r, a); set { b = value.x; b = value.y; r = value.z; a = value.w; } }
    public vec4 bbgr { get => new(b, b, g, r); set { b = value.x; b = value.y; g = value.z; r = value.w; } }
    public vec4 bbgg { get => new(b, b, g, g); set { b = value.x; b = value.y; g = value.z; g = value.w; } }
    public vec4 bbgb { get => new(b, b, g, b); set { b = value.x; b = value.y; g = value.z; b = value.w; } }
    public vec4 bbga { get => new(b, b, g, a); set { b = value.x; b = value.y; g = value.z; a = value.w; } }
    public vec4 bbbr { get => new(b, b, b, r); set { b = value.x; b = value.y; b = value.z; r = value.w; } }
    public vec4 bbbg { get => new(b, b, b, g); set { b = value.x; b = value.y; b = value.z; g = value.w; } }
    public vec4 bbbb { get => new(b, b, b, b); set { b = value.x; b = value.y; b = value.z; b = value.w; } }
    public vec4 bbba { get => new(b, b, b, a); set { b = value.x; b = value.y; b = value.z; a = value.w; } }
    public vec4 bbar { get => new(b, b, a, r); set { b = value.x; b = value.y; a = value.z; r = value.w; } }
    public vec4 bbag { get => new(b, b, a, g); set { b = value.x; b = value.y; a = value.z; g = value.w; } }
    public vec4 bbab { get => new(b, b, a, b); set { b = value.x; b = value.y; a = value.z; b = value.w; } }
    public vec4 bbaa { get => new(b, b, a, a); set { b = value.x; b = value.y; a = value.z; a = value.w; } }
    public vec4 barr { get => new(b, a, r, r); set { b = value.x; a = value.y; r = value.z; r = value.w; } }
    public vec4 barg { get => new(b, a, r, g); set { b = value.x; a = value.y; r = value.z; g = value.w; } }
    public vec4 barb { get => new(b, a, r, b); set { b = value.x; a = value.y; r = value.z; b = value.w; } }
    public vec4 bara { get => new(b, a, r, a); set { b = value.x; a = value.y; r = value.z; a = value.w; } }
    public vec4 bagr { get => new(b, a, g, r); set { b = value.x; a = value.y; g = value.z; r = value.w; } }
    public vec4 bagg { get => new(b, a, g, g); set { b = value.x; a = value.y; g = value.z; g = value.w; } }
    public vec4 bagb { get => new(b, a, g, b); set { b = value.x; a = value.y; g = value.z; b = value.w; } }
    public vec4 baga { get => new(b, a, g, a); set { b = value.x; a = value.y; g = value.z; a = value.w; } }
    public vec4 babr { get => new(b, a, b, r); set { b = value.x; a = value.y; b = value.z; r = value.w; } }
    public vec4 babg { get => new(b, a, b, g); set { b = value.x; a = value.y; b = value.z; g = value.w; } }
    public vec4 babb { get => new(b, a, b, b); set { b = value.x; a = value.y; b = value.z; b = value.w; } }
    public vec4 baba { get => new(b, a, b, a); set { b = value.x; a = value.y; b = value.z; a = value.w; } }
    public vec4 baar { get => new(b, a, a, r); set { b = value.x; a = value.y; a = value.z; r = value.w; } }
    public vec4 baag { get => new(b, a, a, g); set { b = value.x; a = value.y; a = value.z; g = value.w; } }
    public vec4 baab { get => new(b, a, a, b); set { b = value.x; a = value.y; a = value.z; b = value.w; } }
    public vec4 baaa { get => new(b, a, a, a); set { b = value.x; a = value.y; a = value.z; a = value.w; } }
    public vec4 arrr { get => new(a, r, r, r); set { a = value.x; r = value.y; r = value.z; r = value.w; } }
    public vec4 arrg { get => new(a, r, r, g); set { a = value.x; r = value.y; r = value.z; g = value.w; } }
    public vec4 arrb { get => new(a, r, r, b); set { a = value.x; r = value.y; r = value.z; b = value.w; } }
    public vec4 arra { get => new(a, r, r, a); set { a = value.x; r = value.y; r = value.z; a = value.w; } }
    public vec4 argr { get => new(a, r, g, r); set { a = value.x; r = value.y; g = value.z; r = value.w; } }
    public vec4 argg { get => new(a, r, g, g); set { a = value.x; r = value.y; g = value.z; g = value.w; } }
    public vec4 argb { get => new(a, r, g, b); set { a = value.x; r = value.y; g = value.z; b = value.w; } }
    public vec4 arga { get => new(a, r, g, a); set { a = value.x; r = value.y; g = value.z; a = value.w; } }
    public vec4 arbr { get => new(a, r, b, r); set { a = value.x; r = value.y; b = value.z; r = value.w; } }
    public vec4 arbg { get => new(a, r, b, g); set { a = value.x; r = value.y; b = value.z; g = value.w; } }
    public vec4 arbb { get => new(a, r, b, b); set { a = value.x; r = value.y; b = value.z; b = value.w; } }
    public vec4 arba { get => new(a, r, b, a); set { a = value.x; r = value.y; b = value.z; a = value.w; } }
    public vec4 arar { get => new(a, r, a, r); set { a = value.x; r = value.y; a = value.z; r = value.w; } }
    public vec4 arag { get => new(a, r, a, g); set { a = value.x; r = value.y; a = value.z; g = value.w; } }
    public vec4 arab { get => new(a, r, a, b); set { a = value.x; r = value.y; a = value.z; b = value.w; } }
    public vec4 araa { get => new(a, r, a, a); set { a = value.x; r = value.y; a = value.z; a = value.w; } }
    public vec4 agrr { get => new(a, g, r, r); set { a = value.x; g = value.y; r = value.z; r = value.w; } }
    public vec4 agrg { get => new(a, g, r, g); set { a = value.x; g = value.y; r = value.z; g = value.w; } }
    public vec4 agrb { get => new(a, g, r, b); set { a = value.x; g = value.y; r = value.z; b = value.w; } }
    public vec4 agra { get => new(a, g, r, a); set { a = value.x; g = value.y; r = value.z; a = value.w; } }
    public vec4 aggr { get => new(a, g, g, r); set { a = value.x; g = value.y; g = value.z; r = value.w; } }
    public vec4 aggg { get => new(a, g, g, g); set { a = value.x; g = value.y; g = value.z; g = value.w; } }
    public vec4 aggb { get => new(a, g, g, b); set { a = value.x; g = value.y; g = value.z; b = value.w; } }
    public vec4 agga { get => new(a, g, g, a); set { a = value.x; g = value.y; g = value.z; a = value.w; } }
    public vec4 agbr { get => new(a, g, b, r); set { a = value.x; g = value.y; b = value.z; r = value.w; } }
    public vec4 agbg { get => new(a, g, b, g); set { a = value.x; g = value.y; b = value.z; g = value.w; } }
    public vec4 agbb { get => new(a, g, b, b); set { a = value.x; g = value.y; b = value.z; b = value.w; } }
    public vec4 agba { get => new(a, g, b, a); set { a = value.x; g = value.y; b = value.z; a = value.w; } }
    public vec4 agar { get => new(a, g, a, r); set { a = value.x; g = value.y; a = value.z; r = value.w; } }
    public vec4 agag { get => new(a, g, a, g); set { a = value.x; g = value.y; a = value.z; g = value.w; } }
    public vec4 agab { get => new(a, g, a, b); set { a = value.x; g = value.y; a = value.z; b = value.w; } }
    public vec4 agaa { get => new(a, g, a, a); set { a = value.x; g = value.y; a = value.z; a = value.w; } }
    public vec4 abrr { get => new(a, b, r, r); set { a = value.x; b = value.y; r = value.z; r = value.w; } }
    public vec4 abrg { get => new(a, b, r, g); set { a = value.x; b = value.y; r = value.z; g = value.w; } }
    public vec4 abrb { get => new(a, b, r, b); set { a = value.x; b = value.y; r = value.z; b = value.w; } }
    public vec4 abra { get => new(a, b, r, a); set { a = value.x; b = value.y; r = value.z; a = value.w; } }
    public vec4 abgr { get => new(a, b, g, r); set { a = value.x; b = value.y; g = value.z; r = value.w; } }
    public vec4 abgg { get => new(a, b, g, g); set { a = value.x; b = value.y; g = value.z; g = value.w; } }
    public vec4 abgb { get => new(a, b, g, b); set { a = value.x; b = value.y; g = value.z; b = value.w; } }
    public vec4 abga { get => new(a, b, g, a); set { a = value.x; b = value.y; g = value.z; a = value.w; } }
    public vec4 abbr { get => new(a, b, b, r); set { a = value.x; b = value.y; b = value.z; r = value.w; } }
    public vec4 abbg { get => new(a, b, b, g); set { a = value.x; b = value.y; b = value.z; g = value.w; } }
    public vec4 abbb { get => new(a, b, b, b); set { a = value.x; b = value.y; b = value.z; b = value.w; } }
    public vec4 abba { get => new(a, b, b, a); set { a = value.x; b = value.y; b = value.z; a = value.w; } }
    public vec4 abar { get => new(a, b, a, r); set { a = value.x; b = value.y; a = value.z; r = value.w; } }
    public vec4 abag { get => new(a, b, a, g); set { a = value.x; b = value.y; a = value.z; g = value.w; } }
    public vec4 abab { get => new(a, b, a, b); set { a = value.x; b = value.y; a = value.z; b = value.w; } }
    public vec4 abaa { get => new(a, b, a, a); set { a = value.x; b = value.y; a = value.z; a = value.w; } }
    public vec4 aarr { get => new(a, a, r, r); set { a = value.x; a = value.y; r = value.z; r = value.w; } }
    public vec4 aarg { get => new(a, a, r, g); set { a = value.x; a = value.y; r = value.z; g = value.w; } }
    public vec4 aarb { get => new(a, a, r, b); set { a = value.x; a = value.y; r = value.z; b = value.w; } }
    public vec4 aara { get => new(a, a, r, a); set { a = value.x; a = value.y; r = value.z; a = value.w; } }
    public vec4 aagr { get => new(a, a, g, r); set { a = value.x; a = value.y; g = value.z; r = value.w; } }
    public vec4 aagg { get => new(a, a, g, g); set { a = value.x; a = value.y; g = value.z; g = value.w; } }
    public vec4 aagb { get => new(a, a, g, b); set { a = value.x; a = value.y; g = value.z; b = value.w; } }
    public vec4 aaga { get => new(a, a, g, a); set { a = value.x; a = value.y; g = value.z; a = value.w; } }
    public vec4 aabr { get => new(a, a, b, r); set { a = value.x; a = value.y; b = value.z; r = value.w; } }
    public vec4 aabg { get => new(a, a, b, g); set { a = value.x; a = value.y; b = value.z; g = value.w; } }
    public vec4 aabb { get => new(a, a, b, b); set { a = value.x; a = value.y; b = value.z; b = value.w; } }
    public vec4 aaba { get => new(a, a, b, a); set { a = value.x; a = value.y; b = value.z; a = value.w; } }
    public vec4 aaar { get => new(a, a, a, r); set { a = value.x; a = value.y; a = value.z; r = value.w; } }
    public vec4 aaag { get => new(a, a, a, g); set { a = value.x; a = value.y; a = value.z; g = value.w; } }
    public vec4 aaab { get => new(a, a, a, b); set { a = value.x; a = value.y; a = value.z; b = value.w; } }
    public vec4 aaaa { get => new(a, a, a, a); set { a = value.x; a = value.y; a = value.z; a = value.w; } }
    public vec4 ssss { get => new(s, s, s, s); set { s = value.x; s = value.y; s = value.z; s = value.w; } }
    public vec4 ssst { get => new(s, s, s, t); set { s = value.x; s = value.y; s = value.z; t = value.w; } }
    public vec4 sssp { get => new(s, s, s, p); set { s = value.x; s = value.y; s = value.z; p = value.w; } }
    public vec4 sssq { get => new(s, s, s, q); set { s = value.x; s = value.y; s = value.z; q = value.w; } }
    public vec4 ssts { get => new(s, s, t, s); set { s = value.x; s = value.y; t = value.z; s = value.w; } }
    public vec4 sstt { get => new(s, s, t, t); set { s = value.x; s = value.y; t = value.z; t = value.w; } }
    public vec4 sstp { get => new(s, s, t, p); set { s = value.x; s = value.y; t = value.z; p = value.w; } }
    public vec4 sstq { get => new(s, s, t, q); set { s = value.x; s = value.y; t = value.z; q = value.w; } }
    public vec4 ssps { get => new(s, s, p, s); set { s = value.x; s = value.y; p = value.z; s = value.w; } }
    public vec4 sspt { get => new(s, s, p, t); set { s = value.x; s = value.y; p = value.z; t = value.w; } }
    public vec4 sspp { get => new(s, s, p, p); set { s = value.x; s = value.y; p = value.z; p = value.w; } }
    public vec4 sspq { get => new(s, s, p, q); set { s = value.x; s = value.y; p = value.z; q = value.w; } }
    public vec4 ssqs { get => new(s, s, q, s); set { s = value.x; s = value.y; q = value.z; s = value.w; } }
    public vec4 ssqt { get => new(s, s, q, t); set { s = value.x; s = value.y; q = value.z; t = value.w; } }
    public vec4 ssqp { get => new(s, s, q, p); set { s = value.x; s = value.y; q = value.z; p = value.w; } }
    public vec4 ssqq { get => new(s, s, q, q); set { s = value.x; s = value.y; q = value.z; q = value.w; } }
    public vec4 stss { get => new(s, t, s, s); set { s = value.x; t = value.y; s = value.z; s = value.w; } }
    public vec4 stst { get => new(s, t, s, t); set { s = value.x; t = value.y; s = value.z; t = value.w; } }
    public vec4 stsp { get => new(s, t, s, p); set { s = value.x; t = value.y; s = value.z; p = value.w; } }
    public vec4 stsq { get => new(s, t, s, q); set { s = value.x; t = value.y; s = value.z; q = value.w; } }
    public vec4 stts { get => new(s, t, t, s); set { s = value.x; t = value.y; t = value.z; s = value.w; } }
    public vec4 sttt { get => new(s, t, t, t); set { s = value.x; t = value.y; t = value.z; t = value.w; } }
    public vec4 sttp { get => new(s, t, t, p); set { s = value.x; t = value.y; t = value.z; p = value.w; } }
    public vec4 sttq { get => new(s, t, t, q); set { s = value.x; t = value.y; t = value.z; q = value.w; } }
    public vec4 stps { get => new(s, t, p, s); set { s = value.x; t = value.y; p = value.z; s = value.w; } }
    public vec4 stpt { get => new(s, t, p, t); set { s = value.x; t = value.y; p = value.z; t = value.w; } }
    public vec4 stpp { get => new(s, t, p, p); set { s = value.x; t = value.y; p = value.z; p = value.w; } }
    public vec4 stpq { get => new(s, t, p, q); set { s = value.x; t = value.y; p = value.z; q = value.w; } }
    public vec4 stqs { get => new(s, t, q, s); set { s = value.x; t = value.y; q = value.z; s = value.w; } }
    public vec4 stqt { get => new(s, t, q, t); set { s = value.x; t = value.y; q = value.z; t = value.w; } }
    public vec4 stqp { get => new(s, t, q, p); set { s = value.x; t = value.y; q = value.z; p = value.w; } }
    public vec4 stqq { get => new(s, t, q, q); set { s = value.x; t = value.y; q = value.z; q = value.w; } }
    public vec4 spss { get => new(s, p, s, s); set { s = value.x; p = value.y; s = value.z; s = value.w; } }
    public vec4 spst { get => new(s, p, s, t); set { s = value.x; p = value.y; s = value.z; t = value.w; } }
    public vec4 spsp { get => new(s, p, s, p); set { s = value.x; p = value.y; s = value.z; p = value.w; } }
    public vec4 spsq { get => new(s, p, s, q); set { s = value.x; p = value.y; s = value.z; q = value.w; } }
    public vec4 spts { get => new(s, p, t, s); set { s = value.x; p = value.y; t = value.z; s = value.w; } }
    public vec4 sptt { get => new(s, p, t, t); set { s = value.x; p = value.y; t = value.z; t = value.w; } }
    public vec4 sptp { get => new(s, p, t, p); set { s = value.x; p = value.y; t = value.z; p = value.w; } }
    public vec4 sptq { get => new(s, p, t, q); set { s = value.x; p = value.y; t = value.z; q = value.w; } }
    public vec4 spps { get => new(s, p, p, s); set { s = value.x; p = value.y; p = value.z; s = value.w; } }
    public vec4 sppt { get => new(s, p, p, t); set { s = value.x; p = value.y; p = value.z; t = value.w; } }
    public vec4 sppp { get => new(s, p, p, p); set { s = value.x; p = value.y; p = value.z; p = value.w; } }
    public vec4 sppq { get => new(s, p, p, q); set { s = value.x; p = value.y; p = value.z; q = value.w; } }
    public vec4 spqs { get => new(s, p, q, s); set { s = value.x; p = value.y; q = value.z; s = value.w; } }
    public vec4 spqt { get => new(s, p, q, t); set { s = value.x; p = value.y; q = value.z; t = value.w; } }
    public vec4 spqp { get => new(s, p, q, p); set { s = value.x; p = value.y; q = value.z; p = value.w; } }
    public vec4 spqq { get => new(s, p, q, q); set { s = value.x; p = value.y; q = value.z; q = value.w; } }
    public vec4 sqss { get => new(s, q, s, s); set { s = value.x; q = value.y; s = value.z; s = value.w; } }
    public vec4 sqst { get => new(s, q, s, t); set { s = value.x; q = value.y; s = value.z; t = value.w; } }
    public vec4 sqsp { get => new(s, q, s, p); set { s = value.x; q = value.y; s = value.z; p = value.w; } }
    public vec4 sqsq { get => new(s, q, s, q); set { s = value.x; q = value.y; s = value.z; q = value.w; } }
    public vec4 sqts { get => new(s, q, t, s); set { s = value.x; q = value.y; t = value.z; s = value.w; } }
    public vec4 sqtt { get => new(s, q, t, t); set { s = value.x; q = value.y; t = value.z; t = value.w; } }
    public vec4 sqtp { get => new(s, q, t, p); set { s = value.x; q = value.y; t = value.z; p = value.w; } }
    public vec4 sqtq { get => new(s, q, t, q); set { s = value.x; q = value.y; t = value.z; q = value.w; } }
    public vec4 sqps { get => new(s, q, p, s); set { s = value.x; q = value.y; p = value.z; s = value.w; } }
    public vec4 sqpt { get => new(s, q, p, t); set { s = value.x; q = value.y; p = value.z; t = value.w; } }
    public vec4 sqpp { get => new(s, q, p, p); set { s = value.x; q = value.y; p = value.z; p = value.w; } }
    public vec4 sqpq { get => new(s, q, p, q); set { s = value.x; q = value.y; p = value.z; q = value.w; } }
    public vec4 sqqs { get => new(s, q, q, s); set { s = value.x; q = value.y; q = value.z; s = value.w; } }
    public vec4 sqqt { get => new(s, q, q, t); set { s = value.x; q = value.y; q = value.z; t = value.w; } }
    public vec4 sqqp { get => new(s, q, q, p); set { s = value.x; q = value.y; q = value.z; p = value.w; } }
    public vec4 sqqq { get => new(s, q, q, q); set { s = value.x; q = value.y; q = value.z; q = value.w; } }
    public vec4 tsss { get => new(t, s, s, s); set { t = value.x; s = value.y; s = value.z; s = value.w; } }
    public vec4 tsst { get => new(t, s, s, t); set { t = value.x; s = value.y; s = value.z; t = value.w; } }
    public vec4 tssp { get => new(t, s, s, p); set { t = value.x; s = value.y; s = value.z; p = value.w; } }
    public vec4 tssq { get => new(t, s, s, q); set { t = value.x; s = value.y; s = value.z; q = value.w; } }
    public vec4 tsts { get => new(t, s, t, s); set { t = value.x; s = value.y; t = value.z; s = value.w; } }
    public vec4 tstt { get => new(t, s, t, t); set { t = value.x; s = value.y; t = value.z; t = value.w; } }
    public vec4 tstp { get => new(t, s, t, p); set { t = value.x; s = value.y; t = value.z; p = value.w; } }
    public vec4 tstq { get => new(t, s, t, q); set { t = value.x; s = value.y; t = value.z; q = value.w; } }
    public vec4 tsps { get => new(t, s, p, s); set { t = value.x; s = value.y; p = value.z; s = value.w; } }
    public vec4 tspt { get => new(t, s, p, t); set { t = value.x; s = value.y; p = value.z; t = value.w; } }
    public vec4 tspp { get => new(t, s, p, p); set { t = value.x; s = value.y; p = value.z; p = value.w; } }
    public vec4 tspq { get => new(t, s, p, q); set { t = value.x; s = value.y; p = value.z; q = value.w; } }
    public vec4 tsqs { get => new(t, s, q, s); set { t = value.x; s = value.y; q = value.z; s = value.w; } }
    public vec4 tsqt { get => new(t, s, q, t); set { t = value.x; s = value.y; q = value.z; t = value.w; } }
    public vec4 tsqp { get => new(t, s, q, p); set { t = value.x; s = value.y; q = value.z; p = value.w; } }
    public vec4 tsqq { get => new(t, s, q, q); set { t = value.x; s = value.y; q = value.z; q = value.w; } }
    public vec4 ttss { get => new(t, t, s, s); set { t = value.x; t = value.y; s = value.z; s = value.w; } }
    public vec4 ttst { get => new(t, t, s, t); set { t = value.x; t = value.y; s = value.z; t = value.w; } }
    public vec4 ttsp { get => new(t, t, s, p); set { t = value.x; t = value.y; s = value.z; p = value.w; } }
    public vec4 ttsq { get => new(t, t, s, q); set { t = value.x; t = value.y; s = value.z; q = value.w; } }
    public vec4 ttts { get => new(t, t, t, s); set { t = value.x; t = value.y; t = value.z; s = value.w; } }
    public vec4 tttt { get => new(t, t, t, t); set { t = value.x; t = value.y; t = value.z; t = value.w; } }
    public vec4 tttp { get => new(t, t, t, p); set { t = value.x; t = value.y; t = value.z; p = value.w; } }
    public vec4 tttq { get => new(t, t, t, q); set { t = value.x; t = value.y; t = value.z; q = value.w; } }
    public vec4 ttps { get => new(t, t, p, s); set { t = value.x; t = value.y; p = value.z; s = value.w; } }
    public vec4 ttpt { get => new(t, t, p, t); set { t = value.x; t = value.y; p = value.z; t = value.w; } }
    public vec4 ttpp { get => new(t, t, p, p); set { t = value.x; t = value.y; p = value.z; p = value.w; } }
    public vec4 ttpq { get => new(t, t, p, q); set { t = value.x; t = value.y; p = value.z; q = value.w; } }
    public vec4 ttqs { get => new(t, t, q, s); set { t = value.x; t = value.y; q = value.z; s = value.w; } }
    public vec4 ttqt { get => new(t, t, q, t); set { t = value.x; t = value.y; q = value.z; t = value.w; } }
    public vec4 ttqp { get => new(t, t, q, p); set { t = value.x; t = value.y; q = value.z; p = value.w; } }
    public vec4 ttqq { get => new(t, t, q, q); set { t = value.x; t = value.y; q = value.z; q = value.w; } }
    public vec4 tpss { get => new(t, p, s, s); set { t = value.x; p = value.y; s = value.z; s = value.w; } }
    public vec4 tpst { get => new(t, p, s, t); set { t = value.x; p = value.y; s = value.z; t = value.w; } }
    public vec4 tpsp { get => new(t, p, s, p); set { t = value.x; p = value.y; s = value.z; p = value.w; } }
    public vec4 tpsq { get => new(t, p, s, q); set { t = value.x; p = value.y; s = value.z; q = value.w; } }
    public vec4 tpts { get => new(t, p, t, s); set { t = value.x; p = value.y; t = value.z; s = value.w; } }
    public vec4 tptt { get => new(t, p, t, t); set { t = value.x; p = value.y; t = value.z; t = value.w; } }
    public vec4 tptp { get => new(t, p, t, p); set { t = value.x; p = value.y; t = value.z; p = value.w; } }
    public vec4 tptq { get => new(t, p, t, q); set { t = value.x; p = value.y; t = value.z; q = value.w; } }
    public vec4 tpps { get => new(t, p, p, s); set { t = value.x; p = value.y; p = value.z; s = value.w; } }
    public vec4 tppt { get => new(t, p, p, t); set { t = value.x; p = value.y; p = value.z; t = value.w; } }
    public vec4 tppp { get => new(t, p, p, p); set { t = value.x; p = value.y; p = value.z; p = value.w; } }
    public vec4 tppq { get => new(t, p, p, q); set { t = value.x; p = value.y; p = value.z; q = value.w; } }
    public vec4 tpqs { get => new(t, p, q, s); set { t = value.x; p = value.y; q = value.z; s = value.w; } }
    public vec4 tpqt { get => new(t, p, q, t); set { t = value.x; p = value.y; q = value.z; t = value.w; } }
    public vec4 tpqp { get => new(t, p, q, p); set { t = value.x; p = value.y; q = value.z; p = value.w; } }
    public vec4 tpqq { get => new(t, p, q, q); set { t = value.x; p = value.y; q = value.z; q = value.w; } }
    public vec4 tqss { get => new(t, q, s, s); set { t = value.x; q = value.y; s = value.z; s = value.w; } }
    public vec4 tqst { get => new(t, q, s, t); set { t = value.x; q = value.y; s = value.z; t = value.w; } }
    public vec4 tqsp { get => new(t, q, s, p); set { t = value.x; q = value.y; s = value.z; p = value.w; } }
    public vec4 tqsq { get => new(t, q, s, q); set { t = value.x; q = value.y; s = value.z; q = value.w; } }
    public vec4 tqts { get => new(t, q, t, s); set { t = value.x; q = value.y; t = value.z; s = value.w; } }
    public vec4 tqtt { get => new(t, q, t, t); set { t = value.x; q = value.y; t = value.z; t = value.w; } }
    public vec4 tqtp { get => new(t, q, t, p); set { t = value.x; q = value.y; t = value.z; p = value.w; } }
    public vec4 tqtq { get => new(t, q, t, q); set { t = value.x; q = value.y; t = value.z; q = value.w; } }
    public vec4 tqps { get => new(t, q, p, s); set { t = value.x; q = value.y; p = value.z; s = value.w; } }
    public vec4 tqpt { get => new(t, q, p, t); set { t = value.x; q = value.y; p = value.z; t = value.w; } }
    public vec4 tqpp { get => new(t, q, p, p); set { t = value.x; q = value.y; p = value.z; p = value.w; } }
    public vec4 tqpq { get => new(t, q, p, q); set { t = value.x; q = value.y; p = value.z; q = value.w; } }
    public vec4 tqqs { get => new(t, q, q, s); set { t = value.x; q = value.y; q = value.z; s = value.w; } }
    public vec4 tqqt { get => new(t, q, q, t); set { t = value.x; q = value.y; q = value.z; t = value.w; } }
    public vec4 tqqp { get => new(t, q, q, p); set { t = value.x; q = value.y; q = value.z; p = value.w; } }
    public vec4 tqqq { get => new(t, q, q, q); set { t = value.x; q = value.y; q = value.z; q = value.w; } }
    public vec4 psss { get => new(p, s, s, s); set { p = value.x; s = value.y; s = value.z; s = value.w; } }
    public vec4 psst { get => new(p, s, s, t); set { p = value.x; s = value.y; s = value.z; t = value.w; } }
    public vec4 pssp { get => new(p, s, s, p); set { p = value.x; s = value.y; s = value.z; p = value.w; } }
    public vec4 pssq { get => new(p, s, s, q); set { p = value.x; s = value.y; s = value.z; q = value.w; } }
    public vec4 psts { get => new(p, s, t, s); set { p = value.x; s = value.y; t = value.z; s = value.w; } }
    public vec4 pstt { get => new(p, s, t, t); set { p = value.x; s = value.y; t = value.z; t = value.w; } }
    public vec4 pstp { get => new(p, s, t, p); set { p = value.x; s = value.y; t = value.z; p = value.w; } }
    public vec4 pstq { get => new(p, s, t, q); set { p = value.x; s = value.y; t = value.z; q = value.w; } }
    public vec4 psps { get => new(p, s, p, s); set { p = value.x; s = value.y; p = value.z; s = value.w; } }
    public vec4 pspt { get => new(p, s, p, t); set { p = value.x; s = value.y; p = value.z; t = value.w; } }
    public vec4 pspp { get => new(p, s, p, p); set { p = value.x; s = value.y; p = value.z; p = value.w; } }
    public vec4 pspq { get => new(p, s, p, q); set { p = value.x; s = value.y; p = value.z; q = value.w; } }
    public vec4 psqs { get => new(p, s, q, s); set { p = value.x; s = value.y; q = value.z; s = value.w; } }
    public vec4 psqt { get => new(p, s, q, t); set { p = value.x; s = value.y; q = value.z; t = value.w; } }
    public vec4 psqp { get => new(p, s, q, p); set { p = value.x; s = value.y; q = value.z; p = value.w; } }
    public vec4 psqq { get => new(p, s, q, q); set { p = value.x; s = value.y; q = value.z; q = value.w; } }
    public vec4 ptss { get => new(p, t, s, s); set { p = value.x; t = value.y; s = value.z; s = value.w; } }
    public vec4 ptst { get => new(p, t, s, t); set { p = value.x; t = value.y; s = value.z; t = value.w; } }
    public vec4 ptsp { get => new(p, t, s, p); set { p = value.x; t = value.y; s = value.z; p = value.w; } }
    public vec4 ptsq { get => new(p, t, s, q); set { p = value.x; t = value.y; s = value.z; q = value.w; } }
    public vec4 ptts { get => new(p, t, t, s); set { p = value.x; t = value.y; t = value.z; s = value.w; } }
    public vec4 pttt { get => new(p, t, t, t); set { p = value.x; t = value.y; t = value.z; t = value.w; } }
    public vec4 pttp { get => new(p, t, t, p); set { p = value.x; t = value.y; t = value.z; p = value.w; } }
    public vec4 pttq { get => new(p, t, t, q); set { p = value.x; t = value.y; t = value.z; q = value.w; } }
    public vec4 ptps { get => new(p, t, p, s); set { p = value.x; t = value.y; p = value.z; s = value.w; } }
    public vec4 ptpt { get => new(p, t, p, t); set { p = value.x; t = value.y; p = value.z; t = value.w; } }
    public vec4 ptpp { get => new(p, t, p, p); set { p = value.x; t = value.y; p = value.z; p = value.w; } }
    public vec4 ptpq { get => new(p, t, p, q); set { p = value.x; t = value.y; p = value.z; q = value.w; } }
    public vec4 ptqs { get => new(p, t, q, s); set { p = value.x; t = value.y; q = value.z; s = value.w; } }
    public vec4 ptqt { get => new(p, t, q, t); set { p = value.x; t = value.y; q = value.z; t = value.w; } }
    public vec4 ptqp { get => new(p, t, q, p); set { p = value.x; t = value.y; q = value.z; p = value.w; } }
    public vec4 ptqq { get => new(p, t, q, q); set { p = value.x; t = value.y; q = value.z; q = value.w; } }
    public vec4 ppss { get => new(p, p, s, s); set { p = value.x; p = value.y; s = value.z; s = value.w; } }
    public vec4 ppst { get => new(p, p, s, t); set { p = value.x; p = value.y; s = value.z; t = value.w; } }
    public vec4 ppsp { get => new(p, p, s, p); set { p = value.x; p = value.y; s = value.z; p = value.w; } }
    public vec4 ppsq { get => new(p, p, s, q); set { p = value.x; p = value.y; s = value.z; q = value.w; } }
    public vec4 ppts { get => new(p, p, t, s); set { p = value.x; p = value.y; t = value.z; s = value.w; } }
    public vec4 pptt { get => new(p, p, t, t); set { p = value.x; p = value.y; t = value.z; t = value.w; } }
    public vec4 pptp { get => new(p, p, t, p); set { p = value.x; p = value.y; t = value.z; p = value.w; } }
    public vec4 pptq { get => new(p, p, t, q); set { p = value.x; p = value.y; t = value.z; q = value.w; } }
    public vec4 ppps { get => new(p, p, p, s); set { p = value.x; p = value.y; p = value.z; s = value.w; } }
    public vec4 pppt { get => new(p, p, p, t); set { p = value.x; p = value.y; p = value.z; t = value.w; } }
    public vec4 pppp { get => new(p, p, p, p); set { p = value.x; p = value.y; p = value.z; p = value.w; } }
    public vec4 pppq { get => new(p, p, p, q); set { p = value.x; p = value.y; p = value.z; q = value.w; } }
    public vec4 ppqs { get => new(p, p, q, s); set { p = value.x; p = value.y; q = value.z; s = value.w; } }
    public vec4 ppqt { get => new(p, p, q, t); set { p = value.x; p = value.y; q = value.z; t = value.w; } }
    public vec4 ppqp { get => new(p, p, q, p); set { p = value.x; p = value.y; q = value.z; p = value.w; } }
    public vec4 ppqq { get => new(p, p, q, q); set { p = value.x; p = value.y; q = value.z; q = value.w; } }
    public vec4 pqss { get => new(p, q, s, s); set { p = value.x; q = value.y; s = value.z; s = value.w; } }
    public vec4 pqst { get => new(p, q, s, t); set { p = value.x; q = value.y; s = value.z; t = value.w; } }
    public vec4 pqsp { get => new(p, q, s, p); set { p = value.x; q = value.y; s = value.z; p = value.w; } }
    public vec4 pqsq { get => new(p, q, s, q); set { p = value.x; q = value.y; s = value.z; q = value.w; } }
    public vec4 pqts { get => new(p, q, t, s); set { p = value.x; q = value.y; t = value.z; s = value.w; } }
    public vec4 pqtt { get => new(p, q, t, t); set { p = value.x; q = value.y; t = value.z; t = value.w; } }
    public vec4 pqtp { get => new(p, q, t, p); set { p = value.x; q = value.y; t = value.z; p = value.w; } }
    public vec4 pqtq { get => new(p, q, t, q); set { p = value.x; q = value.y; t = value.z; q = value.w; } }
    public vec4 pqps { get => new(p, q, p, s); set { p = value.x; q = value.y; p = value.z; s = value.w; } }
    public vec4 pqpt { get => new(p, q, p, t); set { p = value.x; q = value.y; p = value.z; t = value.w; } }
    public vec4 pqpp { get => new(p, q, p, p); set { p = value.x; q = value.y; p = value.z; p = value.w; } }
    public vec4 pqpq { get => new(p, q, p, q); set { p = value.x; q = value.y; p = value.z; q = value.w; } }
    public vec4 pqqs { get => new(p, q, q, s); set { p = value.x; q = value.y; q = value.z; s = value.w; } }
    public vec4 pqqt { get => new(p, q, q, t); set { p = value.x; q = value.y; q = value.z; t = value.w; } }
    public vec4 pqqp { get => new(p, q, q, p); set { p = value.x; q = value.y; q = value.z; p = value.w; } }
    public vec4 pqqq { get => new(p, q, q, q); set { p = value.x; q = value.y; q = value.z; q = value.w; } }
    public vec4 qsss { get => new(q, s, s, s); set { q = value.x; s = value.y; s = value.z; s = value.w; } }
    public vec4 qsst { get => new(q, s, s, t); set { q = value.x; s = value.y; s = value.z; t = value.w; } }
    public vec4 qssp { get => new(q, s, s, p); set { q = value.x; s = value.y; s = value.z; p = value.w; } }
    public vec4 qssq { get => new(q, s, s, q); set { q = value.x; s = value.y; s = value.z; q = value.w; } }
    public vec4 qsts { get => new(q, s, t, s); set { q = value.x; s = value.y; t = value.z; s = value.w; } }
    public vec4 qstt { get => new(q, s, t, t); set { q = value.x; s = value.y; t = value.z; t = value.w; } }
    public vec4 qstp { get => new(q, s, t, p); set { q = value.x; s = value.y; t = value.z; p = value.w; } }
    public vec4 qstq { get => new(q, s, t, q); set { q = value.x; s = value.y; t = value.z; q = value.w; } }
    public vec4 qsps { get => new(q, s, p, s); set { q = value.x; s = value.y; p = value.z; s = value.w; } }
    public vec4 qspt { get => new(q, s, p, t); set { q = value.x; s = value.y; p = value.z; t = value.w; } }
    public vec4 qspp { get => new(q, s, p, p); set { q = value.x; s = value.y; p = value.z; p = value.w; } }
    public vec4 qspq { get => new(q, s, p, q); set { q = value.x; s = value.y; p = value.z; q = value.w; } }
    public vec4 qsqs { get => new(q, s, q, s); set { q = value.x; s = value.y; q = value.z; s = value.w; } }
    public vec4 qsqt { get => new(q, s, q, t); set { q = value.x; s = value.y; q = value.z; t = value.w; } }
    public vec4 qsqp { get => new(q, s, q, p); set { q = value.x; s = value.y; q = value.z; p = value.w; } }
    public vec4 qsqq { get => new(q, s, q, q); set { q = value.x; s = value.y; q = value.z; q = value.w; } }
    public vec4 qtss { get => new(q, t, s, s); set { q = value.x; t = value.y; s = value.z; s = value.w; } }
    public vec4 qtst { get => new(q, t, s, t); set { q = value.x; t = value.y; s = value.z; t = value.w; } }
    public vec4 qtsp { get => new(q, t, s, p); set { q = value.x; t = value.y; s = value.z; p = value.w; } }
    public vec4 qtsq { get => new(q, t, s, q); set { q = value.x; t = value.y; s = value.z; q = value.w; } }
    public vec4 qtts { get => new(q, t, t, s); set { q = value.x; t = value.y; t = value.z; s = value.w; } }
    public vec4 qttt { get => new(q, t, t, t); set { q = value.x; t = value.y; t = value.z; t = value.w; } }
    public vec4 qttp { get => new(q, t, t, p); set { q = value.x; t = value.y; t = value.z; p = value.w; } }
    public vec4 qttq { get => new(q, t, t, q); set { q = value.x; t = value.y; t = value.z; q = value.w; } }
    public vec4 qtps { get => new(q, t, p, s); set { q = value.x; t = value.y; p = value.z; s = value.w; } }
    public vec4 qtpt { get => new(q, t, p, t); set { q = value.x; t = value.y; p = value.z; t = value.w; } }
    public vec4 qtpp { get => new(q, t, p, p); set { q = value.x; t = value.y; p = value.z; p = value.w; } }
    public vec4 qtpq { get => new(q, t, p, q); set { q = value.x; t = value.y; p = value.z; q = value.w; } }
    public vec4 qtqs { get => new(q, t, q, s); set { q = value.x; t = value.y; q = value.z; s = value.w; } }
    public vec4 qtqt { get => new(q, t, q, t); set { q = value.x; t = value.y; q = value.z; t = value.w; } }
    public vec4 qtqp { get => new(q, t, q, p); set { q = value.x; t = value.y; q = value.z; p = value.w; } }
    public vec4 qtqq { get => new(q, t, q, q); set { q = value.x; t = value.y; q = value.z; q = value.w; } }
    public vec4 qpss { get => new(q, p, s, s); set { q = value.x; p = value.y; s = value.z; s = value.w; } }
    public vec4 qpst { get => new(q, p, s, t); set { q = value.x; p = value.y; s = value.z; t = value.w; } }
    public vec4 qpsp { get => new(q, p, s, p); set { q = value.x; p = value.y; s = value.z; p = value.w; } }
    public vec4 qpsq { get => new(q, p, s, q); set { q = value.x; p = value.y; s = value.z; q = value.w; } }
    public vec4 qpts { get => new(q, p, t, s); set { q = value.x; p = value.y; t = value.z; s = value.w; } }
    public vec4 qptt { get => new(q, p, t, t); set { q = value.x; p = value.y; t = value.z; t = value.w; } }
    public vec4 qptp { get => new(q, p, t, p); set { q = value.x; p = value.y; t = value.z; p = value.w; } }
    public vec4 qptq { get => new(q, p, t, q); set { q = value.x; p = value.y; t = value.z; q = value.w; } }
    public vec4 qpps { get => new(q, p, p, s); set { q = value.x; p = value.y; p = value.z; s = value.w; } }
    public vec4 qppt { get => new(q, p, p, t); set { q = value.x; p = value.y; p = value.z; t = value.w; } }
    public vec4 qppp { get => new(q, p, p, p); set { q = value.x; p = value.y; p = value.z; p = value.w; } }
    public vec4 qppq { get => new(q, p, p, q); set { q = value.x; p = value.y; p = value.z; q = value.w; } }
    public vec4 qpqs { get => new(q, p, q, s); set { q = value.x; p = value.y; q = value.z; s = value.w; } }
    public vec4 qpqt { get => new(q, p, q, t); set { q = value.x; p = value.y; q = value.z; t = value.w; } }
    public vec4 qpqp { get => new(q, p, q, p); set { q = value.x; p = value.y; q = value.z; p = value.w; } }
    public vec4 qpqq { get => new(q, p, q, q); set { q = value.x; p = value.y; q = value.z; q = value.w; } }
    public vec4 qqss { get => new(q, q, s, s); set { q = value.x; q = value.y; s = value.z; s = value.w; } }
    public vec4 qqst { get => new(q, q, s, t); set { q = value.x; q = value.y; s = value.z; t = value.w; } }
    public vec4 qqsp { get => new(q, q, s, p); set { q = value.x; q = value.y; s = value.z; p = value.w; } }
    public vec4 qqsq { get => new(q, q, s, q); set { q = value.x; q = value.y; s = value.z; q = value.w; } }
    public vec4 qqts { get => new(q, q, t, s); set { q = value.x; q = value.y; t = value.z; s = value.w; } }
    public vec4 qqtt { get => new(q, q, t, t); set { q = value.x; q = value.y; t = value.z; t = value.w; } }
    public vec4 qqtp { get => new(q, q, t, p); set { q = value.x; q = value.y; t = value.z; p = value.w; } }
    public vec4 qqtq { get => new(q, q, t, q); set { q = value.x; q = value.y; t = value.z; q = value.w; } }
    public vec4 qqps { get => new(q, q, p, s); set { q = value.x; q = value.y; p = value.z; s = value.w; } }
    public vec4 qqpt { get => new(q, q, p, t); set { q = value.x; q = value.y; p = value.z; t = value.w; } }
    public vec4 qqpp { get => new(q, q, p, p); set { q = value.x; q = value.y; p = value.z; p = value.w; } }
    public vec4 qqpq { get => new(q, q, p, q); set { q = value.x; q = value.y; p = value.z; q = value.w; } }
    public vec4 qqqs { get => new(q, q, q, s); set { q = value.x; q = value.y; q = value.z; s = value.w; } }
    public vec4 qqqt { get => new(q, q, q, t); set { q = value.x; q = value.y; q = value.z; t = value.w; } }
    public vec4 qqqp { get => new(q, q, q, p); set { q = value.x; q = value.y; q = value.z; p = value.w; } }
    public vec4 qqqq { get => new(q, q, q, q); set { q = value.x; q = value.y; q = value.z; q = value.w; } }
}