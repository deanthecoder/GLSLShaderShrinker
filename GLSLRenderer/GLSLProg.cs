// -----------------------------------------------------------------------
//  <copyright file="GLSLProg.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace GLSLRenderer;

[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
[SuppressMessage("ReSharper", "JoinDeclarationAndInitializer")]
[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
public partial class GLSLProg : GLSLProgBase
{
	vec3 lp = vec3(6, 3, -10);
	int progressive_time;
	float t,
		fade = 1.0f;
	vec2 g = new();
	vec4 m = new();
	bool hitGlass;

	struct Hit
	{
		public Hit(float d_, float id_, vec3 p_)
		{
			d = d_;
			id = id_;
			p = Clone(ref p_);
		}

		public float d, id;
		public vec3 p;
	};

	void U(ref Hit h, float d, float id, vec3 p)
	{
		Clone(ref p);
		if (d < h.d) h = new Hit(d, id, p);
	}

	float max3(vec3 v)
	{
		Clone(ref v);
		return max(v.x, max(v.y, v.z));
	}

	float dot3(vec3 v)
	{
		Clone(ref v);
		return dot(v, v);
	}

	float sum2(vec2 v)
	{
		Clone(ref v);
		return dot(v, vec2(1));
	}

	float sum3(vec3 v)
	{
		Clone(ref v);
		return dot(v, vec3(1));
	}

	float mul2(vec2 v)
	{
		Clone(ref v);
		return v.x * v.y;
	}

	vec2 h22(vec2 p)
	{
		Clone(ref p);
		vec3 v = fract(p.xyx * vec3(.1031f, .103f, .0973f));
		v += dot(v, v.yzx + 333.33f);
		return fract((v.xx + v.yz) * v.zy);
	}

	float h31(vec3 p3)
	{
		Clone(ref p3);
		p3 = fract(p3 * .1031f);
		p3 += dot(p3, p3.yzx + 333.3456f);
		return fract(sum2(p3.xy) * p3.z);
	}

	float h21(vec2 p)
	{
		Clone(ref p);
		return h31(p.xyx);
	}

	float n31(vec3 p)
	{
		Clone(ref p);
		vec3 s = vec3(7, 157, 113);
		vec3 ip = floor(p);
		p = fract(p);
		p = p * p * (3.0f - 2.0f * p);
		vec4 h = vec4(0, s.yz, sum2(s.yz)) + dot(ip, s);
		h = mix(fract(sin(h) * 43758.545f), fract(sin(h + s.x) * 43758.545f), p.x);
		h.xy = mix(h.xz, h.yw, p.y);
		return mix(h.x, h.y, p.z);
	}

	vec2 n331(vec3 p)
	{
		Clone(ref p);
		vec2 s = vec2(20, 38);
		vec2 ns = new();
		for (int i = 0; i < 2; i++)
			ns[i] = n31(p * s[i]);

		return ns;
	}

	float fbm(vec3 p)
	{
		Clone(ref p);
		int octaves = 4;
		float roughness = .5f,
			sum = 0.0f,
			amp = 1.0f,
			tot = 0.0f;
		roughness = clamp(roughness, 0.0f, 1.0f);
		while (octaves-- > 0)
		{
			sum += amp * n31(p);
			tot += amp;
			amp *= roughness;
			p *= 2.0f;
		}

		return sum / tot;
	}

	float voronoi(vec2 p)
	{
		Clone(ref p);
		vec2 o = new(),
			g = floor(p);
		p -= g;
		vec3 d = vec3(2);
		for (int y = -1; y <= 1; y++)
		{
			for (int x = -1; x <= 1; x++)
			{
				o = vec2(x, y);
				o += h22(g + o) - p;
				d.z = dot(o, o);
				d.y = max(d.x, min(d.y, d.z));
				d.x = min(d.x, d.z);
			}
		}

		return d.y - d.x;
	}

	mat2 rot(float a)
	{
		float c = cos(a),
			s = sin(a);
		return mat2(c, s, -s, c);
	}

	vec3 dy(vec3 p)
	{
		Clone(ref p);
		p.y += -1.6f;
		return p;
	}

	float boxFrame(vec3 p)
	{
		Clone(ref p);
		p = abs(p) - vec3(1);
		vec3 q = abs(p + .1f) - .1f,
			v1 = vec3(q.xz, p.y),
			v2 = vec3(q.xy, p.z),
			v3 = vec3(q.yz, p.x);
		return min(min(length(max(v3, 0.0f)) + min(max3(v3), 0.0f), length(max(v1, 0.0f)) + min(max3(v1), 0.0f)), length(max(v2, 0.0f)) + min(max3(v2), 0.0f));
	}

	float tor(vec3 p)
	{
		Clone(ref p);
		vec2 t = vec2(2, .2f);
		vec2 q = vec2(length(p.xz) - t.x, p.y);
		return length(q) - t.y;
	}

	vec3 rayDir(vec3 ro, vec2 uv)
	{
		Clone(ref uv);
		Clone(ref ro);
		vec3 f = normalize(vec3(0) - ro),
			r = normalize(cross(vec3(0, 1, 0), f));
		return normalize(f + r * uv.x + cross(f, r) * uv.y);
	}

	vec3 skyCol(float y)
	{
		vec3 c = pow(vec3(max(1.0f - y * .5f, 0.0f)), vec3(6, 3, 1.5f)) * vec3(.95f, 1, 1) * (1.0f - pow(vec3(2), -vec3(35, 14, 7)));
		c *= mix(vec3(1), vec3(1, .5f, .4f), sin(t * .2f) * .5f + .5f);
		return c;
	}

	vec3 sky(vec3 rd)
	{
		Clone(ref rd);
		vec3 p = new(),
			col = skyCol(rd.y);
		float den,
			d = 10.0f / rd.y;
		if (d < 0.0f) return col;
		p = rd * d + vec3(1, .2f, 1) * t * .2f;
		p.xz *= .2f;
		den = 1.0f;
		for (int i = 0; i < 3; i++)
			den *= exp(-.06f * fbm(p));

		return mix(col, vec3(2, 1.6f, 1.4f), smoothstep(.9f, 1.0f, den) * (1.0f - clamp(d / 64.0f, 0.0f, 1.0f)));
	}

	float fakeEnv(vec3 n)
	{
		Clone(ref n);
		return length(sin(n * 2.5f) * .5f + .5f) / 1.73205f;
	}

	float glassSdf(vec3 p)
	{
		Clone(ref p);
		p.y += 2.0f;
		return tor(dy(p));
	}

	Hit sdf(vec3 p)
	{
		Clone(ref p);
		Hit h = new Hit(length(p - vec3(0, .5f, 0)) - 1.0f, 4.3f, p);
		U(ref h, boxFrame(p + vec3(0, 1, 0)) - .02f, 2.3f, p);
		if (p.y < 0.0)
			U(ref h, p.y + 2.0f + smoothstep(.1f, 0.0f, voronoi(p.xz)) * .01f, 3.3f, p);
		if (!hitGlass) U(ref h, glassSdf(p), 5.0f, p);
		h.d -= .01f;
		return h;
	}

	vec3 N(vec3 p, float t)
	{
		Clone(ref p);
		float h = t * .05f;
		vec3 n = vec3(0);
		for (int i = min(iFrame, 0); i < 4; i++)
		{
			vec3 e = .005773f * (2.0f * vec3(((i + 3) >> 1) & 1, (i >> 1) & 1, i & 1) - 1.0f);
			n += e * sdf(p + e * h).d;
		}

		return normalize(n);
	}

	vec3 NGlass(vec3 p)
	{
		Clone(ref p);
		vec3 n = vec3(0);
		for (int i = min(iFrame, 0); i < 4; i++)
		{
			vec3 e = .005773f * (2.0f * vec3(((i + 3) >> 1) & 1, (i >> 1) & 1, i & 1) - 1.0f);
			n += e * glassSdf(p + e * .01f);
		}

		return normalize(n);
	}

	float shadow(vec3 p, vec3 ld, vec3 n)
	{
		Clone(ref n);
		Clone(ref ld);
		Clone(ref p);
		if (dot(ld, n) < -.1f) return 0.0f;
		float d,
			s = 1.0f,
			t = .01f,
			mxt = length(p - lp);
		for (float i = min(iTime, 0.0f); i < 30.0f; i++)
		{
			d = sdf(t * ld + p).d;
			s = min(s, mix(50.0f, 7.0f, .8f) * d / t);
			t += max(.05f, d);
			if (mxt - t < .5f || s < .001f) break;
		}

		return smoothstep(0.0f, 1.0f, s);
	}

	float ao(vec3 p, vec3 n)
	{
		Clone(ref n);
		Clone(ref p);
		vec2 h = vec2(.2f, 1);
		vec2 ao = new();
		for (int i = min(iFrame, 0); i < 2; i++)
			ao[i] = sdf(h[i] * n + p).d;

		return clamp(mul2(.5f + .5f * ao / h), 0.0f, 1.0f);
	}

	float sss(vec3 p, vec3 ld)
	{
		Clone(ref ld);
		Clone(ref p);
		float h = .1f;
		return smoothstep(0.0f, 1.0f, sdf(h * ld + p).d / h);
	}

	vec3 lights(vec3 p, vec3 ro, vec3 rd, vec3 n, Hit h, float fogAdjust)
	{
		Clone(ref h);
		Clone(ref n);
		Clone(ref rd);
		Clone(ref ro);
		Clone(ref p);
		if (h.id == 0.0f) return sky(rd);
		float _ao,
			sha,
			fre,
			fogY,
			fg,
			fogTex,
			ss = 0.0f,
			shine = 1.0f;
		vec3 c = new(),
			l = new(),
			col = new(),
			uv = new(),
			skyTop = new(),
			ld = normalize(lp - p);
		vec2 ns = n331(h.p);
		if (h.id == 4.3f) c = vec3(.5f - sum2(ns) * .05f);
		else if (h.id == 2.3f)
		{
			shine = 1.0f - sum2(ns) * .1f;
			c = vec3(.5f, .4f, .3f) * shine;
			ss = sss(p, -ld) * .5f;
		}
		else c = vec3(.2f);

		if (progressive_time == 0) return vec3(sum3(c) / 3.0f);
		if (progressive_time == 1) return c;
		_ao = ao(p, n);
		if (progressive_time == 2) return c * _ao;
		sha = shadow(p, ld, n);
		if (progressive_time == 3) return (c + ss) * (.2f + .8f * sha) * _ao;
		l = clamp(vec3(dot(ld, n), dot(-ld.xz, n.xz), n.y), 0.0f, 1.0f);
		l.x *= fakeEnv(ld * 4.0f);
		l.x += ss;
		l.xy = .1f + .9f * l.xy;
		l.yz *= _ao;
		l *= vec3(1, .5f, .2f);
		l.x *= .1f + .9f * sha;
		skyTop = skyCol(1.0f);
		c += skyTop * (1.0f - sha) * .5f;
		shine *= .5f + .5f * ns.x * ns.y;
		shine *= sha;
		l.x += pow(clamp(dot(normalize(ld - rd), n), 0.0f, 1.0f), 10.0f) * shine;
		l.x *= dot(lp, lp) / (1.0f + dot(lp - p, lp - p));
		fre = smoothstep(.6f, 1.0f, 1.0f + dot(rd, n)) * .25f;
		col = mix((sum2(l.xy) * vec3(2, 1.6f, 1.4f) + l.z * skyTop) * c, skyTop, fre);
		if (progressive_time < 6) return col;
		fogY = -1.0f;
		fogTex = fbm(vec3(p.xz, fogY) * .4f + t * vec3(.1f, -.9f, .2f));
		fogY -= (1.0f - fogTex) * .3f;
		fg = smoothstep(0.0f, -.3f, p.y - fogY);
		fg *= .1f + .2f * fogTex;
		fg *= 1.0f - clamp(-rd.y, 0.0f, 1.0f);
		fg += 1.0f - exp(dot3(p - ro) / -fogAdjust * mix(1e-4f, .01f, .1f));
		return mix(skyCol(0.0f), col, 1.0f - clamp(fg, 0.0f, 1.0f));
	}

	float addFade()
	{
		return min(1.0f, abs(t));
	}

	vec3 march(vec3 ro, vec3 rd)
	{
		Clone(ref rd);
		Clone(ref ro);
		t = mod(iTime, 30.0f);
		fade = addFade();
		vec3 glassN = new(),
			dv = new(),
			p = ro,
			col = vec3(0),
			glassP = col;
		float i,
			d = 1.0f;
		hitGlass = false;
		Hit h = new();
		for (i = min(iTime, 0.0f); i < 120.0f; i++)
		{
			if (d > 64.0f)
			{
				h.id = 0.0f;
				break;
			}

			h = sdf(p);
			if (abs(h.d) < 2e-4f * d)
			{
				if (!hitGlass && h.id == 5.0f)
				{
					hitGlass = true;
					glassP = p;
					glassN = NGlass(p);
					rd = normalize(refract(rd, glassN, .76f));
					continue;
				}

				break;
			}

			d += h.d;
			p += h.d * rd;
		}

		dv = rd;
		if (progressive_time > 5)
		{
			for (i = 1.5f; i < d; i += 4.0f)
			{
				vec3 vp = ro + dv * i;
				vp.yz -= t * .05f;
				g.x += .3f * (1.0f - smoothstep(0.0f, mix(.05f, .02f, clamp((i - 1.0f) / 19.0f, 0.0f, 1.0f)), length(fract(vp - ro) - .5f)));
				dv.xz *= mat2(.87758f, .47943f, -.47943f, .87758f);
			}

		}

		vec3 n = new();
		col += g.x * vec3(2, 1.6f, 1.4f);
		col += lights(p, ro, rd, n = N(p, d), h, 1.0f);
		if (progressive_time > 4)
		{
			if (fract(h.id) > 0.0f || hitGlass)
			{
				if (hitGlass)
				{
					p = glassP;
					n = glassN;
					vec3 ld = normalize(lp - p);
					col += .05f + vec3(2, 1.6f, 1.4f) * pow(clamp(dot(normalize(ld - rd), n), 0.0f, 1.0f), 80.0f);
					col += .1f * clamp(glassSdf(p + normalize(ld - p) * .2f) / .2f, 0.0f, 1.0f);
				}

				float opac = 1.0f;
				for (float bounce = 0.0f; bounce < 2.0f && opac > .1f; bounce++)
				{
					float refl = fract(h.id);
					rd = reflect(rd, n);
					p += n * .01f;
					ro = p;
					d = .01f;
					for (i = min(iTime, 0.0f); i < 64.0f; i++)
					{
						if (d > 20.0f)
						{
							h.id = 0.0f;
							bounce = 2.0f;
							break;
						}

						h = sdf(p);
						if (abs(h.d) < 2e-4f * d) break;
						d += h.d;
						p += h.d * rd;
					}

					opac *= refl;
					col += opac * (1.0f - col) * lights(p, ro, rd, n = N(p, d), h, .2f);
				}
			}
		}

		return pow(max(vec3(0), col), vec3(.4545f));
	}

	void mainImage(out vec4 fragColor, vec2 fc)
	{
		Clone(ref fc);
		g = vec2(0);
		m = abs(iMouse / vec2(640, 360).xyxy);
		vec2 uv = (fc - .5f * iResolution.xy) / iResolution.y,
			v = fc.xy / iResolution.xy;
		vec3 col = new(),
			ro = vec3(0, .001f, -6);
		float pt = max(0.0f, iTime - 1.0f - uv.x + uv.y * .2f) / 3.0f;
		if (pt > .1f && pt < 7.0f && fract(pt) < .01f)
		{
			fragColor = vec4(1);
			return;
		}

		progressive_time = toInt(pt);

		ro += .1f * sin(iTime * vec3(.9f, .7f, .3f));

		ro.yz *= rot(m.y * 2.0f - 1.0f);
		ro.xz *= rot(m.x * 2.0f - 1.0f);

		col = march(ro, rayDir(ro, uv));

		col *= 1.0f - .3f * dot(uv, uv);
		col += (h21(fc) - .5f) / 128.0f;
		fragColor = vec4(col * fade, 0);
	}
}