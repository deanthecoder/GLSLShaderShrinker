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
public class GLSLProg : GLSLProgBase
{
	struct H
	{
		public H(float d_, vec3 mat_, float spe_)
		{
			d = d_;
			mat = Clone(ref mat_);
			spe = spe_;
		}

		public float d;
		public vec3 mat;
		public float spe;
	};

	float smin(float a, float b, float k)
	{
		float h = clamp(.50f + .50f * (b - a) / k, 0.0f, 1.0f);
		return mix(b, a, h) - k * h * (1.0f - h);
	}

	float B(vec3 p, vec3 b)
	{
		Clone(ref b);
		Clone(ref p);
		return length(max(abs(p) - b, 0.0f));
	}

	float cap(vec3 p, float h, float r)
	{
		Clone(ref p);
		vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(h, r);
		return min(max(d.x, d.y), 0.0f) + length(max(d, 0.0f));
	}

	float P(vec3 p)
	{
		Clone(ref p);
		p.y -= 3.20f;
		float d = max(length(p.xz) - .30f, p.y);
		d = smin(d, length(p) - .80f, .10f);
		p.y += 1.050f;
		d = smin(d, cap(p, .750f, .070f), .50f);
		p.y += 1.50f;
		d = smin(d, cap(p, .750f, .120f), .50f);
		p.y += .40f;
		return smin(d, cap(p, .90f, .20f), .40f);
	}

	float K(vec3 p)
	{
		Clone(ref p);
		p.y -= 1.850f;
		float d = cap(p, .40f - .140f * cos(p.y * 1.40f - .80f), 2.0f);
		p.y--;
		d = smin(d, cap(p, .70f, .10f), .20f);
		p.y += 2.0f;
		d = smin(d, cap(p, .70f, .10f), .20f);
		p.y += .50f;
		d = smin(d, cap(p, 1.0f, .30f), .10f);
		p.xz *= mat2(.764840f, -.644220f, .644220f, .764840f);
		p.y -= 4.0f;
		return min(min(d, B(p, vec3(.50f, .20f, .10f))), B(p, vec3(.20f, .50f, .10f)));
	}

	H map(vec3 p)
	{
		Clone(ref p);
		H h = new H(P(p * 1.20f), vec3(.80f), 20.0f);
		float gnd = length(p.y);
		if (gnd < h.d)
		{
			h.d = gnd;
			h.mat = vec3(.20f);
		}

		return h;
	}

	vec3 no(vec3 p, float t)
	{
		Clone(ref p);
		vec2 e = vec2(.57730f, -.57730f) * t * .0030f;
		return normalize(e.xyy * map(p + e.xyy).d + e.yyx * map(p + e.yyx).d + e.yxy * map(p + e.yxy).d + e.xxx * map(p + e.xxx).d);
	}

	float sha(vec3 p)
	{
		Clone(ref p);
		vec3 rd = normalize(vec3(-8, 8, -8) - p);
		float res = 1.0f,
			t = .50f;
		for (float h, i = 0.0f; i < 32.0f; i++)
		{
			h = K(p + rd * t);
			res = min(res, 150.0f * h / t);
			t += h;
			if (res < .010f || t > 20.0f) break;
		}

		return clamp(res, 0.0f, 1.0f);
	}

	float ao(vec3 p, vec3 n, float h)
	{
		Clone(ref n);
		Clone(ref p);
		return map(p + h * n).d / h;
	}

	vec3 applyLighting(vec3 p, vec3 rd, float d, H data)
	{
		Clone(ref data);
		Clone(ref rd);
		Clone(ref p);
		vec3 l = normalize(vec3(-8, 8, -8) - p),
			n = no(p, d);
		float _ao = dot(vec3(ao(p, n, .20f), ao(p, n, .50f), ao(p, n, 2.0f)), vec3(.20f, .30f, .50f)),
			primary = max(0.0f, dot(l, n)),
			bounce = max(0.0f, dot(l * vec3(-1, 0, -1), n)) * .10f,
			spe = smoothstep(0.0f, 1.0f, pow(max(0.0f, dot(rd, reflect(l, n))), data.spe));
		primary *= mix(.40f, 1.0f, sha(p));
		return data.mat * ((primary + bounce) * _ao + spe) * vec3(2, 1.60f, 1.40f) * exp(-length(p) * .10f);
	}

	vec3 rgb(vec3 ro, vec3 rd)
	{
		Clone(ref rd);
		Clone(ref ro);
		vec3 p = new();
		float d = .010f;
		H h = new();
		for (float steps = 0.0f; steps < 45.0f; steps++)
		{
			p = ro + rd * d;
			h = map(p);
			if (abs(h.d) < .00150f) break;
			if (d > 64.0f) return vec3(0);
			d += h.d;
		}

		return applyLighting(p, rd, d, h);
	}

	void mainImage(out vec4 fragColor, vec2 fc)
	{
		Clone(ref fc);
		vec3 f = new(),
			r = new(),
			ro = vec3(2, 5, -10),
			col = vec3(0),
			R = iResolution;
		ro.yz *= mat2(.980070f, -.198670f, .198670f, .980070f);
		f = normalize(vec3(2, 4, 0) - ro);
		r = normalize(cross(vec3(0, 1, 0), f));
		for (float dx = 0.0f; dx <= 1.0f; dx++)
		{
			for (float dy = 0.0f; dy <= 1.0f; dy++)
			{
				vec2 uv = ((fc + vec2(dx, dy) * .50f) - .50f * R.xy) / R.y;
				col += rgb(ro, normalize(f + r * uv.x + cross(f, r) * uv.y));
			}
		}

		col = pow(col / 4.0f, vec3(.450f));
		vec2 q = fc.xy / R.xy;
		col *= .50f + .50f * pow(16.0f * q.x * q.y * (1.0f - q.x) * (1.0f - q.y), .40f);
		fragColor = vec4(col, 1);
	}

	public GLSLProg(vec2 resolution)
		: base(resolution, 20.0f)
	{
	}
}