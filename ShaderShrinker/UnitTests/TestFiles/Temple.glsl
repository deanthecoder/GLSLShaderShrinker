// https://www.shadertoy.com/view/td2yzd
// Thanks to all the experts out there that spend their time
// writing blogs, YouTube videos, and documenting their shader code!
// Iq, BigWings, P_Malin, Shane, and many others. Thanks!
//
// The appearance of this scene is inspired by Iq's AWESOME temple (https://www.shadertoy.com/view/ldScDh),
// but with the code written from scratch (as much as I can..)
//
//   -Dean

#define MAX_DIST 1200.
#define MAX_STEPS 200
#define ZERO min(iFrame, 0)

#define SUNCOL vec3(1.64, 1.27, .99)
#define SKYCOL vec3(.6, .8, .9)

#define SEA_LEVEL -100.

float hash(vec2 p) {
	return fract(sin(dot(p, vec2(123.45, 875.43))) * 5432.3);
}

// Smooth noise generator.
float noise(vec2 p) {
	vec2 i = floor(p),
		 f = fract(p);

	float a = hash(i),
		  b = hash(i + vec2(1, 0)),
		  c = hash(i + vec2(0, 1)),
		  d = hash(i + vec2(1, 1));
	f = f * f * (3. - 2. * f); // smoothstep with no clamp.

	return mix(a, b, f.x) +
		   (c - a) * f.y * (1. - f.x) +
		   (d - b) * f.x * f.y;
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

vec2 repeat(vec2 p, vec2 gap) {
	return mod(p, gap) - gap / 2.;
}

float sdCube(vec3 p, vec3 r) {
	p = abs(p) - r;
  	return length(max(p, 0.)) + min(max(p.x, max(p.y, p.z)), 0.);
}

float sdColumn(vec3 p, float r) {
	return length(p.xz) - r;
}

float sdTempleColumn(vec3 p) {
	float h = 8.,
		  r = .6,
		  baseHeight = .16;

	// Column.
	r -= .1 * (p.y / (h / 2.)); // Taper.
	r -= .05 * pow(.5 + .5 * sin(p.y * 4.5), 200.); // Segments.
	r -= .03 * pow(.5 + .5 * sin(15. * atan(p.z, p.x)), 4.); // Ridges.
	float d = sdColumn(p, r);

	// Top block.
	{
		vec3 q = p;
		q.y = q.y - h / 2. + baseHeight;
		d = min(d, sdCube(q, vec3(.7, baseHeight, .7)) - .05);
	}

	// Base block.
	{
		vec3 q = p;
		q.y = q.y + h / 2. - baseHeight * 2.5;
		d = min(d, sdCube(q, vec3(.7, baseHeight, .7)) - .05);
	}

	return d;
}

float sdTemple(vec3 p) {
	float d = 1E10;

	vec3 q = p;
	q.xz = repeat(q.xz, vec2(2)); // x,z repeat for tiles.

	// All columns.
	{
		d = sdTempleColumn(q);
		d = max(d, -sdCube(p, vec3(4, 50, 6))); // Exclude inside.
		d = max(d, sdCube(p, vec3(6, 4, 8))); // Exclude outside.
	}

	// Floor tiles.
	{
		q.y += 4.;
		d = min(d, sdCube(q, vec3(.9, .16, .9)) - .05);
		d = max(d, sdCube(p, vec3(8, 5, 8))); // Exclude outside.
	}

	// Foundation blocks.
	{
		vec3 qq = p;
		qq.xz -= vec2(1);
		qq.xz = repeat(qq.xz, vec2(2));
		qq.y += 5.3;
		d = min(d, sdCube(qq, vec3(.9, 1, .9)) - .05);
		d = max(d, sdCube(p, vec3(9, 6, 9))); // Exclude outside.

		// Roof blocks.
		{
			qq.y = p.y - 4.6;

			float dd = sdCube(qq, vec3(.9, .5, .85)) - .05;
			dd = max(dd, sdCube(p, vec3(6.5, 6, 9)) - .05); // Exclude outside.
			d = min(d, dd);
		}
	}

	// Roof base.
	d = min(d, sdCube(p - vec3(0, 5.3, 0), vec3(7, .2, 9.5) - .05));

	d += fbm((p.xy + p.yz) * 1.2) * .1;
	return d * .8;
}

float sdTerrain(vec3 p) {
	p.y -= SEA_LEVEL;
	float h = 70. * (pow(fbm(p.xz * vec2(.015)), 1.5) - .5),

		  t2 = .7 * fbm((p.xz + vec2(0, 140)) * vec2(.05)) - .538,
		  hAtTemple = -SEA_LEVEL + t2 * 20.;

	h = mix(h, hAtTemple, clamp(smoothstep(10., 0., length(p.xz) / 15.), 0., 1.));

	return p.y - h;
}

float sdSea(vec3 p) {
	float waveHeight = (.5 - fbm(p.xz * .04)) * 2.;
	return p.y - SEA_LEVEL + waveHeight;
}

vec2 map(vec3 p) {
	float o1 = sdTemple(p - vec3(0, 2.35, 0)),
		  o2 = sdTerrain(p),
		  o3 = sdSea(p);

	vec2 hit = vec2(o1, 1); // Temple
	if (o2 < hit.r)
		hit = vec2(o2, 2); // Terrain
	if (o3 < hit.r)
		hit = vec2(o3, 3); // Sea

	return hit;
}

vec3 calcNormal(vec3 p) {
	// Thanks community! I didn't fancy deriving this...
	vec3 n = vec3(0);
	for (int i = ZERO; i < 4; i++)
	{
		vec3 e = .5773*(2.0*vec3((((i+3)>>1)&1),((i>>1)&1),(i&1)) - 1.);
		n += e * map(p + .0005 * e).x;
	}

	return normalize(n * .5773);
}

float calcShadow(vec3 origin, vec3 lightOrigin, float fuzziness) {
	float s = 1.;

	vec3 rayDir = normalize(lightOrigin - origin);
	float d = 1.;
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
	vec2 uv = (fragCoord - .5 * iResolution.xy) / iResolution.y;

	vec3 cam = vec3(7, 6.5, -25),
		 lookAt = vec3(-13, -4, 0),
		 forward = normalize(lookAt - cam),
		 right = normalize(cross(vec3(0, 1, 0), forward)),
		 up = cross(forward, right),
		 rayDir = normalize(forward + uv.x * right + uv.y * up);

	vec3 sunPosUnit = normalize(vec3(.5, .8, .7)),
		 sunPos = sunPosUnit * 100.,
		 ambCol = vec3(.3, .3, .95);

	vec3 col = vec3(0);
	float d = 0.;
	vec2 hit = vec2(0);
	vec3 p;
	for (int steps = ZERO; steps < MAX_STEPS; steps++) {
		p = cam + rayDir * d;

		hit = map(p);
		float distToObj = hit.x;
		if (abs(distToObj) < d * .001) {
			// Ray Hit!
			break;
		}

		if (d > MAX_DIST) {
			hit = vec2(0);
			break;
		}

		d += distToObj;
	}

	if (hit.y < .5) {
		// ID0 Sky
		col = SKYCOL;
	} else {
		vec3 mat,
			 n = calcNormal(p);
		float occ = calcOcclusion(p, n),
			  sha;

		if (hit.y < 1.5) {
			// ID1 Temple
			sha = calcShadow(p, sunPos, .125);
			mat = vec3(.2, .18, .1);
		} else if (hit.y < 2.5) {
			// ID2 Terrain
			float grassy = smoothstep(.7, 1., n.y);
			vec3 rockCol = vec3(.04, .04, .03),
				 grassCol = vec3(.025, .1, .03);
			mat = mix(rockCol, grassCol, grassy);
			sha = calcShadow(p, sunPos, 1.);
		} else if (hit.y < 3.5) {
			// ID3 Sea
			mat = vec3(.002, .02, .04);
			sha = calcShadow(p, sunPos, 1.);
			float terrainDist = sdTerrain(p);
			mat *= 1. * (1. + smoothstep(30., 0., terrainDist)); // Deep sea.
			mat *= 1. * (1. + smoothstep(5., 0., terrainDist)); // Shallow areas.
		}

		float sun = max(0., dot(sunPosUnit, n)) * 6.,
			  back = .2 * max(0., dot(sunPosUnit * vec3(-1, 0, -1), n)),
			  sky = (.6 + .5 * n.y) * .05;

		col = mat * sun * SUNCOL * sha;
		col += mat * back * SUNCOL * occ;
		col += mat * sky * SKYCOL * occ;

		float fog = pow(length(p - cam) / MAX_DIST, 7.);
		col = mix(col, SKYCOL, fog);
	}

	// Output to screen
	col = pow(col, vec3(.4545)); // Gamma correction
	col = vignette(col, fragCoord); // Fade screen corners
	fragColor = vec4(col, 1);
}
