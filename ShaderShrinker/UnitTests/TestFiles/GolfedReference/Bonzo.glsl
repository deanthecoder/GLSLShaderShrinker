// Processed by 'GLSL Shader Shrinker' (Shrunk by 303 characters)
// (https://github.com/deanthecoder/GLSLShaderShrinker)

#version 410 core

precision mediump float;
uniform float a;
uniform vec2 e;
uniform sampler1D t;
layout(location = 0)out vec4 o;
vec4 p(vec2 v, float b) {
	float c = .5 + sin(v.x * 10.) + cos(sin(b + v.y) * 20.);
	return vec4(sin(c * .2 + cos(b)), c * .15, cos(c * .1 + b / .4) * .25, 1);
}

void main() {
	vec2 m,
	     u = vec2(gl_FragCoord.x / e.x, gl_FragCoord.y / e.y);
	u -= .5;
	u /= vec2(e.y / e.x, 1);
	m.x = atan(u.x / u.y) / 3.14;
	m.y = 1 / length(u) * .2;
	float d = m.y,
	      f = texture(t, d).r * 100;
	m.x += sin(a) * .1;
	m.y += a * .25;
	o = f + clamp(p(m * 3.14, a) / d, 0., 1.);
}