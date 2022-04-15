// Processed by 'GLSL Shader Shrinker' (Shrunk by 198 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#version 410 core

precision mediump float;
uniform float fGlobalTime;
uniform vec2 v2Resolution;
uniform sampler1D texFFT;
layout(location = 0)out vec4 o;
vec4 p(vec2 v, float t) {
	float c = .5 + sin(v.x * 10.) + cos(sin(t + v.y) * 20.);
	return vec4(sin(c * .2 + cos(t)), c * .15, cos(c * .1 + t / .4) * .25, 1);
}

void main() {
	vec2 m,
	     u = vec2(gl_FragCoord.x / v2Resolution.x, gl_FragCoord.y / v2Resolution.y);
	u -= .5;
	u /= vec2(v2Resolution.y / v2Resolution.x, 1);
	m.x = atan(u.x / u.y) / 3.14;
	m.y = 1 / length(u) * .2;
	float d = m.y,
	      f = texture(texFFT, d).r * 100;
	m.x += sin(fGlobalTime) * .1;
	m.y += fGlobalTime * .25;
	o = f + clamp(p(m * 3.14, fGlobalTime) / d, 0., 1.);
}