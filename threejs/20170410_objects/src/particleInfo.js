


/* note **

value = 247.36

value to vec4(
  0.0,
  0.96625
)


*/
const FloatToVec4_for_GLSL = `
vec4 FloatToVec4( float value) {
  // return vec4(value >> 24, value >> 16, value >> 8, value);

  const vec4 bitSh = vec4(256.0*256.0*256.0, 256.0*256.0, 256.0, 1.0);
  const vec4 bitMsk = vec4(0.0, 1.0/256.0, 1.0/256.0, 1.0/256.0);
  vec4 res = fract(value * bitSh);
  res -= res.xxyz * bitMsk;
  return res;
}
`;

const Vec4ToFloat_for_GLSL = `
//
float Vec4ToFloat(const vec4 value) {
  const vec4 bitSh = vec4(1.0/(256.0*256.0*256.0), 1.0/(256.0*256.0), 1.0/256.0, 1.0);
  return(dot(value, bitSh));
}
`


class ParticleVelocity {
  constructor(cnt, rdrr) {
    const option = {minFilter : THREE.NearestFilter, magFilter : THREE.NearestFilter };
    this.y = new THREE.WebGLRenderTarget(cnt, 1, option);
    this.x = new THREE.WebGLRenderTarget(cnt, 1, option);
    this.z = new THREE.WebGLRenderTarget(cnt, 1, option);
  }
  update(dt) {

  }
}


class ParticlePosition {
  constructor(cnt, rdrr) {
    const option = {minFilter : THREE.NearestFilter, magFilter : THREE.NearestFilter };
    this.rdrr = rdrr;

    this.x = new THREE.WebGLRenderTarget(cnt, 1, option);
    this.y = new THREE.WebGLRenderTarget(cnt, 1, option);
    this.z = new THREE.WebGLRenderTarget(cnt, 1, option);
    this.t = new THREE.WebGLRenderTarget(cnt, 1, option);

    this.ps = { type : "t" , value : this.t };
    this.vc = { type : "t",  value : this.t };
    this.dt = { type : "1f", value : 0.0 };

    this.s = new THREE.Scene();
    this.c = new THREE.Camera();

    this.initCanvas = new THREE.Mesh(
      new THREE.PlaneGeometry(2.0, 2.0),
      new THREE.ShaderMaterial({
        transparent : true,
        fragmentShader : `
        varying vec2 vtex;
        void main(void) {
          gl_FragColor = vec4(0.0, vtex,  1.0);
        }
        `,
        vertexShader : `
        varying vec2 vtex;
        void main(void) {
          vtex = uv;
          gl_Position = vec4(position, 1.0);
        }
        `
      })
    );

    this.updateCanvas = new THREE.Mesh(
      new THREE.PlaneGeometry(2.0, 2.0),
      new THREE.ShaderMaterial({
        uniforms : {
          unif_position : this.ps,
          unif_velocity : this.vc,
          unif_deltatime : this.dt,
        },
        fragmentShader : `
        uniform sampler2D unif_position;
        uniform sampler2D unif_velocity;
        uniform float unif_deltatime;

        ` + FloatToVec4_for_GLSL + `
        ` + Vec4ToFloat_for_GLSL + `

        varying vec2 vtex;
        const int cnst_particlecnt = ` + cnt + `;
        void main(void) {
          vec4 col = texture2D(unif_position, vtex);
          float pos = Vec4ToFloat(col);
          vec4 retcol = FloatToVec4(pos);
          vec4 retcold = vec4(retcol.r, retcol.g, retcol.b, 1.0);
          gl_FragColor = retcold;//retcol;//vec4(retcol.rgb, 1.0);
        }
        `,
        vertexShader : `
        varying vec2 vtex;
        void main(void) {
          vtex = uv;
          gl_Position = vec4(position, 1.0);
        }
        `
      })
    );

    this.s.add(this.initCanvas);
    this.rdrr.render(this.s, this.c, this.x);
    this.rdrr.render(this.s, this.c, this.y);
    this.rdrr.render(this.s, this.c, this.z);

    this.s.remove(this.initCanvas);
    this.s.add(this.updateCanvas);

  }

  swap(a, b) { var t = a; a = b; b = t; }

  update(dt, velocity) {
    const {rdrr, s, c, x, y, z, t, ps, vc} = this;

    rdrr.autoClear = true;

    ps.value = x; vc.value = velocity.x;
    rdrr.render(s, c, t); this.swap(x, t);

    ps.value = y; vc.value = velocity.y;
    rdrr.render(s, c, t); this.swap(y, t);

    ps.value = z; vc.value = velocity.z;
    rdrr.render(s, c, t); this.swap(z, t);

    rdrr.render(s, c);
    // console.log(rdrr);

    rdrr.autoClear = false;
  }
}
