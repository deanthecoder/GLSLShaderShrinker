// golfing  1307 chars "White Drops" by Xor. https://shadertoy.com/view/fdXXWH

#define A            2                                        // antialiasing A×A
#define T            iTime
#define B(d)       ( dot(d,L) *.5 + .5 )                      // background
#define smin(a,b)   -log( exp(-(a)*6.) + exp(-(b)*6.) ) / 6.

float D(vec3 p) {                                             // shape
    float l = pow( dot(p.xz,p.xz), .8 );
    return smin(  p.y +.8 +.4* sin(l*3.-T+.5) / (1.+l),
                  smin( length( p + vec3(0,1.2,0) * -sin(T)   ) -.4 ,
                        length( p + vec3(.1,.8,0) * cos(T+.1) ) -.2
               )      );
}

void mainImage( out vec4 O, vec2 U ) {
    vec3 R = iResolution, 
         e = vec3(1,-1,0)*.01, 
         L = vec3(.55,.7,.45),                                // light dir
         p = vec3( .05*cos(T), .1*sin(T), -4 ), d,n,m;        // viewer position
    O-=O;
 
    for(int k = 0; k < A*A; k++ )   {                         // ray marching (repeated for AA samples
        m = p; float s = 1., l = 0.;
        d = normalize( vec3( U - R.xy/2. + vec2(k%A,k/A)/vec2(A), R.y ) ); // ray dir     
        for(int i = 0; i++<99 && s>.01 && l<20.; m += d*s )
            l += s = D(m);
            
        n = normalize( D(m-e.yxx)*e.yxx                       // normal at hit point
                     + D(m-e.xyx)*e.xyx
                     + D(m-e.xxy)*e.xxy
                     + D(m-e.y  )*e.y); 
                                                              // shading:
        O += mix( .1* smoothstep(.8,1.,dot(reflect(d,n),L))   // surface specular
                    * sqrt(abs(dot(d,n)))                     // attenuated at silhouette ?
                  + B(refract(d,n,.75))  ,                    // shading = environment map                  
                  B(d) *.5 +.5 ,                              // background
                  pow( min(l/20.,1.), .3 )                    // fogging with distance
                );
    }
    O /= vec4(A*A); O*=O;
}