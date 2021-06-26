// Processed by 'GLSL Shader Shrinker' (Shrunk by 377 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#define SKYCOL	vec3(.6, .8, .9)
#define SEA_LEVEL	- 1e2

float hash(vec2 p) { return fract(sin(dot(p, vec2(123.45, 875.43))) * 5432.3); }

float noise(vec2 p) {
	vec2 i = floor(p),
	     f = fract(p);
	float a = hash(i),
	      b = hash(i + vec2(1, 0)),
	      c = hash(i + vec2(0, 1)),
	      d = hash(i + vec2(1));
	f = f * f * (3. - 2. * f);
	return mix(a, b, f.x) + (c - a) * f.y * (1. - f.x) + (d - b) * f.x * f.y;
}

float fbm(vec2 p) {
	float s = 0.,
	      a = .498,
	      f = 1.012;
	for (int i = 0; i < 8; i++) {
		s += a * noise(p * f);
		a *= .501;
		f *= 1.988;
	}

	return s;
}

vec2 repeat(vec2 p, vec2 gap) { return mod(p, gap) - gap / 2.; }

float sdCube(vec3 p, vec3 r) {
	p = abs(p) - r;
	return length(max(p, 0.)) + min(max(p.x, max(p.y, p.z)), 0.);
}

float sdColumn(vec3 p, float r) { return length(p.xz) - r; }

float sdTempleColumn(vec3 p) {
	float d,
	      r = .6;
	r -= .1 * p.y / 4.;
	r -= .05 * pow(.5 + .5 * sin(p.y * 4.5), 2e2);
	r -= .03 * pow(.5 + .5 * sin(15. * atan(p.z, p.x)), 4.);
	d = sdColumn(p, r);
	{
		vec3 q = p;
		q.y -= 4. + .16;
		d = min(d, sdCube(q, vec3(.7, .16, .7)) - .05);
	}
	{
		vec3 q = p;
		q.y += 4. - .4;
		d = min(d, sdCube(q, vec3(.7, .16, .7)) - .05);
	}
	return d;
}

float sdTemple(vec3 p) {
	float d = 1E10;
	vec3 q = p;
	q.xz = repeat(q.xz, vec2(2));
	{ d = max(max(sdTempleColumn(q), -sdCube(p, vec3(4, 50, 6))), sdCube(p, vec3(6, 4, 8))); }
	{
		q.y += 4.;
		d = max(min(d, sdCube(q, vec3(.9, .16, .9)) - .05), sdCube(p, vec3(8, 5, 8)));
	}
	{
		vec3 qq = p;
		qq.xz -= vec2(1);
		qq.xz = repeat(qq.xz, vec2(2));
		qq.y += 5.3;
		d = max(min(d, sdCube(qq, vec3(.9, 1, .9)) - .05), sdCube(p, vec3(9, 6, 9)));
		{
			qq.y = p.y - 4.6;
			d = min(d, max(sdCube(qq, vec3(.9, .5, .85)) - .05, sdCube(p, vec3(6.5, 6, 9)) - .05));
		}
	}
	d = min(d, sdCube(p - vec3(0, 5.3, 0), vec3(7, .2, 9.5) - .05));
	d += fbm((p.xy + p.yz) * 1.2) * .1;
	return d * .8;
}

float sdTerrain(vec3 p) {
	p.y -= SEA_LEVEL;
	return p.y - mix(70. * (pow(fbm(p.xz * vec2(.015)), 1.5) - .5), -SEA_LEVEL + (.7 * fbm((p.xz + vec2(0, 140)) * vec2(.05)) - .538) * 20., clamp(smoothstep(10., 0., length(p.xz) / 15.), 0., 1.));
}

float sdSea(vec3 p) { return p.y - SEA_LEVEL + (.5 - fbm(p.xz * .04)) * 2.; }

vec2 map(vec3 p) {
	float o1 = sdTemple(p - vec3(0, 2.35, 0)),
	      o2 = sdTerrain(p),
	      o3 = sdSea(p);
	vec2 hit = vec2(o1, 1);
	if (o2 < hit.r) hit = vec2(o2, 2);
	if (o3 < hit.r) hit = vec2(o3, 3);
	return hit;
}

vec3 calcNormal(vec3 p) {
	vec3 n = vec3(0);
	for (int i = min(iFrame, 0); i < 4; i++) {
		vec3 e = .5773 * (2. * vec3(((i + 3) >> 1) & 1, (i >> 1) & 1, i & 1) - 1.);
		n += e * map(p + 5e-4 * e).x;
	}

	return normalize(n * .5773);
}

float calcShadow(vec3 origin, vec3 lightOrigin, float fuzziness) {
	float s = 1.,
	      d = 1.;
	vec3 rayDir = normalize(lightOrigin - origin);
	while (d < 20. && s > 0.) {
		float distToObj = map(origin + rayDir * d).x;
		s = min(s, .5 + .5 * distToObj / (fuzziness * d));
		d += clamp(distToObj, .2, 1.);
	}
	return smoothstep(0., 1., s);
}

float calcOcclusion(vec3 origin, vec3 n) {
	float occ = 0.,
	      d = .01;
	for (int i = 1; i < 5; i++) {
		float h = map(origin + n * d).x;
		occ += d - h;
		d += .15;
	}

	return 1. - clamp(occ * .5, 0., 1.);
}

vec3 vignette(vec3 col, vec2 fragCoord) {
	vec2 q = fragCoord.xy / iResolution.xy;
	col *= .5 + .5 * pow(16. * q.x * q.y * (1. - q.x) * (1. - q.y), .4);
	return col;
}

void mainImage(out vec4 fragColor, vec2 fragCoord) {
	vec2 hit = vec2(0),
	     uv = (fragCoord - .5 * iResolution.xy) / iResolution.y;
	vec3 p,
	     forward = normalize(vec3(-13, -4, 0) - vec3(7, 6.5, -25)),
	     right = normalize(cross(vec3(0, 1, 0), forward)),
	     rayDir = normalize(forward + uv.x * right + uv.y * cross(forward, right)),
	     sunPosUnit = normalize(vec3(.5, .8, .7)),
	     sunPos = sunPosUnit * 1e2,
	     col = vec3(0);
	float d = 0.;
	for (int steps = min(iFrame, 0); steps < 200; steps++) {
		p = vec3(7, 6.5, -25) + rayDir * d;
		hit = map(p);
		float distToObj = hit.x;
		if (abs(distToObj) < d * .001) break;
		if (d > 12e2) {
			hit = vec2(0);
			break;
		}

		d += distToObj;
	}

	if (hit.y < .5) col = SKYCOL;
	else {
		vec3 mat,
		     n = calcNormal(p);
		float sha, sun, back, sky,
		      occ = calcOcclusion(p, n);
		if (hit.y < 1.5) {
			sha = calcShadow(p, sunPos, .125);
			mat = vec3(.2, .18, .1);
		}
		else if (hit.y < 2.5) {
			mat = mix(vec3(.04, .04, .03), vec3(.025, .1, .03), smoothstep(.7, 1., n.y));
			sha = calcShadow(p, sunPos, 1.);
		}
		else if (hit.y < 3.5) {
			mat = vec3(.002, .02, .04);
			sha = calcShadow(p, sunPos, 1.);
			float terrainDist = sdTerrain(p);
			mat *= 1. * (1. + smoothstep(30., 0., terrainDist));
			mat *= 1. * (1. + smoothstep(5., 0., terrainDist));
		}

		sun = max(0., dot(sunPosUnit, n)) * 6.;
		back = .2 * max(0., dot(sunPosUnit * vec3(-1, 0, -1), n));
		sky = (.6 + .5 * n.y) * .05;
		col = mat * sun * vec3(1.64, 1.27, .99) * sha;
		col += mat * back * vec3(1.64, 1.27, .99) * occ;
		col += mat * sky * SKYCOL * occ;
		col = mix(col, SKYCOL, pow(length(p - vec3(7, 6.5, -25)) / 12e2, 7.));
	}

	fragColor = vec4(vignette(pow(col, vec3(.4545)), fragCoord), 1);
}