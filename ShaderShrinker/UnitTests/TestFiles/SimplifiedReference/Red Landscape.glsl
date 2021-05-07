vec3 vec_tovec3(vec3 a) { return a.xyz; }

vec3 noise_vec2_xyx(vec2 self) { return self.xyx; }

vec2 demos_redlandscape_vec3_xz(vec3 self) { return self.xz; }

float vec_smootherstep(float edge0, float edge1, float x) {
	float t = clamp((x - edge0) / (edge1 - edge0), 0., 1.);
	return t * t * t * (t * (t * 6. - 15.) + 10.);
}

float vec_fastmix(float a, float b, float t) { return a + (b - a) * t; }

vec3 vec_fastmix_1(vec3 a, vec3 b, float t) { return a + (b - a) * t; }

vec2 sincos(float x) { return vec2(sin(x), cos(x)); }

vec2 demos_redlandscape_vec3_xy(vec3 self) { return self.xy; }

vec3 noise_vec3_yzx(vec3 self) { return self.yzx; }

vec2 noise_vec3_xx(vec3 self) { return self.xx; }

vec2 noise_vec3_yz(vec3 self) { return self.yz; }

vec2 noise_vec3_zy(vec3 self) { return self.zy; }

float noise_hash1_2(vec2 v) {
	vec3 v3 = noise_vec2_xyx(v);
	v3 = fract(v3 * .1031);
	v3 += dot(v3, (noise_vec3_yzx(v3) + 33.33));
	return fract((v3.x + v3.y) * v3.z);
}

vec2 noise_hash2_1(vec2 v) {
	vec3 v3 = noise_vec2_xyx(v);
	v3 *= vec3(.1031, .103, .0973);
	v3 += dot(v3, (noise_vec3_yzx(v3) + 33.33));
	return fract((noise_vec3_xx(v3) + noise_vec3_yz(v3)) * noise_vec3_zy(v3));
}

float noise_noisemix2(float a, float b, float c, float d, vec2 f) {
	vec2 u = f * f * (3. - 2. * f);
	return vec_fastmix(vec_fastmix(a, b, u.x), vec_fastmix(c, d, u.x), u.y);
}

float noise_noise_white_1(vec2 p) { return noise_hash1_2(p); }

float noise_noise_value_1(vec2 p) {
	vec2 i = floor(p),
	     f = fract(p),
	     I = floor(i + 1.);
	return noise_noisemix2(noise_hash1_2(i), noise_hash1_2(vec2(I.x, i.y)), noise_hash1_2(vec2(i.x, I.y)), noise_hash1_2(I), f);
}

float noise_noise_gradient_1(vec2 p) {
	vec2 i = floor(p),
	     f = fract(p),
	     I = floor(i + 1.),
	     F = f - 1.;
	return .5 + noise_noisemix2(dot((-.5 + noise_hash2_1(i)), f), dot((-.5 + noise_hash2_1(vec2(I.x, i.y))), vec2(F.x, f.y)), dot((-.5 + noise_hash2_1(vec2(i.x, I.y))), vec2(f.x, F.y)), dot((-.5 + noise_hash2_1(I)), F), f);
}

vec3 colorgrade_colorgrade_tonemap_aces(vec3 col) { return clamp(col * (2.51 * col + .03) / (col * (2.43 * col + .59) + .14), 0., 1.); }

vec3 colorgrade_colorgrade_saturate(vec3 col, float sat) {
	float grey = dot(col, vec3(.2125, .7154, .0721));
	return grey + sat * (col - grey);
}

vec3 colorgrade_colorgrade_tone_1(vec3 col, vec3 gain, vec3 lift, vec3 invgamma) {
	col = pow(col, vec_tovec3(invgamma));
	return (gain - lift) * col + lift;
}

vec3 colorgrade_colorgrade_gamma_correction(vec3 col) { return 1.12661 * sqrt(col) - .12661 * col; }

vec3 colorgrade_colorgrade_vignette(vec3 col, vec2 coord, float strength, float amount) { return col * ((1. - amount) + amount * pow(16. * coord.x * coord.y * (1. - coord.x) * (1. - coord.y), strength)); }

vec3 colorgrade_colorgrade_dither(vec3 col, vec2 coord, float amount) { return clamp((col + noise_noise_white_1(coord) * amount), 0., 1.); }

vec3 camera_camera_perspective(vec3 lookfrom, vec3 lookat, float tilt, float vfov, vec2 uv) {
	vec2 sc = sincos(tilt);
	vec3 vup = vec3(sc, 0),
	     w = normalize(lookat - lookfrom),
	     u = normalize(cross(w, vup));
	return normalize((uv.x * u + uv.y * cross(u, w)) + 1. / tan(vfov * .00872664626) * w);
}

float demos_redlandscape_fbm_terrain(vec2 p) {
	float a = 1.,
	      t = 0.;
	t += a * noise_noise_value_1(p);
	a *= .5;
	p = 2. * p;
	t += a * noise_noise_value_1(p);
	a *= .5;
	p = 2. * p;
	t += a * noise_noise_value_1(p);
	a *= .5;
	p = 2. * p;
	t += a * noise_noise_value_1(p);
	a *= .5;
	p = 2. * p;
	return t;
}

float demos_redlandscape_map(vec3 p) { return ((p.y + demos_redlandscape_fbm_terrain(demos_redlandscape_vec3_xz(p)) * .375) + 0.) * .5; }

float demos_redlandscape_ray_march(vec3 ro, vec3 rd) {
	float t = 0.;
	for (int i = 1; i <= 256; i = i + 1) {
		vec3 p = ro + t * rd;
		float d = demos_redlandscape_map(p);
		if (d < .003 * t || t >= 25.) break;
		t += d;
	}
	return t;
}

void mainImage(out vec4 frag_col, vec2 frag_coord) {
	const vec3 hor_col = vec3(.7, .05, .01),
	           sun_col = vec3(.9, .8, .7);
	vec2 sc,
	     res = demos_redlandscape_vec3_xy(iResolution),
	     uv = frag_coord / res,
	     coord = 2. * (frag_coord - res * .5) / res.y;
	float y, t,
	      z = iTime;
	sc = sincos(iTime * .5);
	y = 0.;
	vec3 back_col,
	     lookat = vec3(sc.x * .5, y, z),
	     ro = vec3((-sc.x) * .5, y, (z - 2.)),
	     rd = camera_camera_perspective(ro, lookat, 0., 45., coord),
	     col = vec3(0),
	     sun_dir = normalize(vec3(.3, .07, 1));
	t = demos_redlandscape_ray_march(ro, rd);
	{
		back_col = vec_fastmix_1(vec_fastmix_1(hor_col, hor_col * .3, vec_smootherstep(0., .25, rd.y)), vec3(.8, .3, .1), max((.1 - rd.y), 0.));
		float sun_lightness = max(dot(rd, sun_dir), 0.);
		back_col = ((back_col + sun_col * pow(sun_lightness, 2e3)) + .3 * sun_col * pow(sun_lightness, 1e2)) + vec3(.3, .2, .1) * pow(sun_lightness, 4.);
	}
	if (abs(coord.y) > .75) col = vec3(0);
	else if (t < 25.) {
		float decay = 1. - exp(-.12 * t);
		col = mix(col, back_col, decay);
	}
	else {
		col = back_col;
		float clouds_dist = (1. - ro.y / 1e3) / rd.y;
		if (clouds_dist > 0.) {
			vec2 clouds_pos = demos_redlandscape_vec3_xz(ro) + demos_redlandscape_vec3_xz(rd) * clouds_dist;
			float clouds_lightness = max((noise_noise_gradient_1(clouds_pos) - .3), 0.),
			      clouds_decay = vec_smootherstep(0., .3, rd.y);
			vec3 clouds_col = 2. * col;
			col = vec_fastmix_1(col, clouds_col, clouds_lightness * clouds_decay);
		}
		col = clamp(col, 0., 1.);
	}
	col = colorgrade_colorgrade_dither(colorgrade_colorgrade_vignette(colorgrade_colorgrade_saturate(colorgrade_colorgrade_tone_1(colorgrade_colorgrade_gamma_correction(colorgrade_colorgrade_tonemap_aces(col)), vec3(1.3, .9, .7), vec3(.5, .1, .1) * .1, vec3(3, 2, 1.2)), .7), uv, .25, .7), frag_coord, .01);
	frag_col = vec4(col, 1);
}