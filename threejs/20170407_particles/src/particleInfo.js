


/* note **

value = 247.36

value to vec4(
  0.0,
  0.96625
)


*/
class ParticleSetter {
  constructor(rdrr) {
    this.u = { unif_seed : { type : "4f", value : [Math.random(), Math.random(), Math.random(), Math.random()]}}
    this.rdrr = rdrr;

    this.r = new THREE.Scene();
    this.c = new THREE.Camera();

    this.m = new THREE.Mesh(
      new THREE.PlaneGeometry(2.0, 2.0),
      new THREE.ShaderMaterial({
        transparent : true,
        // side : THREE.DoubleSide,
        uniforms : this.u,
        fragmentShader : `
        uniform vec4 unif_seed;

        float rand(vec2 co){
          return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
        }
        varying vec2 vtex;

        void main(void) {
          gl_FragColor = vec4(
            rand(vtex + unif_seed.xy),
            rand(vtex + unif_seed.yz ),
            rand(vtex + unif_seed.zw),
            rand(vtex + unif_seed.wx));
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
    this.r.add(this.m);
  }

  initTarget(t) {
    this.u.unif_seed.value = [Math.random(), Math.random(), Math.random(), Math.random()];
    this.rdrr.render(this.r, this.c, t);
  }
}

class ParticleLife {
  constructor(cnt, rdrr) {
    this.setter = new ParticleSetter(rdrr);
    const option = {minFilter : THREE.NearestFilter, magFilter : THREE.NearestFilter,  };
    this.rdrr = rdrr;


    const width_ = Math.min(cnt, 16384);
    const height_ = Math.min(Math.floor(cnt / 16384) + 1, 16384);

    this.time = new THREE.WebGLRenderTarget(width_, height_, option);
    this.temp = new THREE.WebGLRenderTarget(width_, height_, option);

    this.li = { type : "t",  value : this.time};
    this.dt = { type : "1f", value : 0.0 };

    this.s = new THREE.Scene();
    this.r = new THREE.Scene();
    this.c = new THREE.Camera();

    this.r.add(new THREE.Mesh(
      new THREE.PlaneGeometry(2.0, 2.0),
      new THREE.MeshBasicMaterial({ map : this.temp })
    ));

    this.canvas = new THREE.Mesh(
      new THREE.PlaneGeometry(2.0, 2.0),
      new THREE.ShaderMaterial({
        uniforms : {
          unif_life : this.li,
          unif_deltatime : this.dt },
        fragmentShader : m_float + `
        uniform sampler2D unif_life;
        uniform float unif_deltatime;

        ` + FloatToVec4_for_GLSL + `
        ` + Vec4ToFloat_for_GLSL + `

        float rand(vec2 co){
          return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
        }

        vec4 randtex(vec2 co) {
          return vec4(//0.1, 0.0, 0.0, 0.0);
            rand(co * (unif_deltatime * 10.0 + 0.01)),
            rand(co * (unif_deltatime * 20.0 + 0.02)),
            rand(co * (unif_deltatime * 30.0 + 0.03)),
            rand(co * (unif_deltatime * 40.0 + 0.04)));
        }

        varying vec2 vtex;
        void main(void) {
          vec4 col = texture2D(unif_life, vtex);
          float lif = Vec4ToFloat(col);
          vec4 retcol;

          lif = ((lif - M_FLOAT) * 2.0) * 255.0;

          if(lif < 0.0) lif = abs(Vec4ToFloat(randtex(vtex)));


          lif -= unif_deltatime * 0.5;
          lif = (lif / 255.0 * 0.5 + M_FLOAT);
          retcol = FloatToVec4(lif);
          // vec4 retcold = texture2D(lif, vtex);
          gl_FragColor = vec4(retcol.rgb, 1.0);//retcol;//vec4(retcol.rgb, 1.0);
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

    this.setter.initTarget(this.x);

    this.s.add(this.canvas);
  }

  _update_texture(dt) {
    const {rdrr, r, s, c, temp, time} = this;
    this.dt.value = dt;
    rdrr.render(s, c, temp);
    rdrr.render(r, c, time);
  }

  update(dt) {
    this._update_texture(dt);
  }
}

class ParticleVelocity {
  constructor(cnt, rdrr, tex) {
    this.setter = new ParticleSetter(rdrr);
    const option = {minFilter : THREE.NearestFilter, magFilter : THREE.NearestFilter,  };
    this.rdrr = rdrr;


    const width_ = Math.min(cnt, 16384);
    const height_ = Math.min(Math.floor(cnt / 16384) + 1, 16384);

    this.x = new THREE.WebGLRenderTarget(width_, height_, option);
    this.y = new THREE.WebGLRenderTarget(width_, height_, option);
    this.z = new THREE.WebGLRenderTarget(width_, height_, option);
    this.t = new THREE.WebGLRenderTarget(width_, height_, option);

    this.dt = { type : "1f", value : 0.0 };

    this.s = new THREE.Scene();
    this.r = new THREE.Scene();
    this.c = new THREE.Camera();

    this.r.add(new THREE.Mesh(
      new THREE.PlaneGeometry(2.0, 2.0),
      new THREE.MeshBasicMaterial({ map : this.t })
    ));

    this.canvas = new THREE.Mesh(
      new THREE.PlaneGeometry(2.0, 2.0),
      new THREE.ShaderMaterial({
        uniforms : {
          unif_deltatime : this.dt,
        },
        fragmentShader : m_float + `
        uniform float unif_deltatime;

        ` + FloatToVec4_for_GLSL + `
        ` + Vec4ToFloat_for_GLSL + `

        varying vec2 vtex;
        void main(void) {
          vec4 col = texture2D(unif_position, vtex);
          float pos = Vec4ToFloat(col);

          pos = ((pos - M_FLOAT) * 2.0) * 255.0;
          pos += 0.3;
          pos = (pos / 255.0 * 0.5 + M_FLOAT);

          vec4 retcol = FloatToVec4(pos);
          vec4 retcold = texture2D(unif_position, vtex);
          gl_FragColor = vec4(retcol.rgb, 1.0);//retcol;//vec4(retcol.rgb, 1.0);
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

    this.setter.initTarget(this.x);
    this.setter.initTarget(this.y);
    this.setter.initTarget(this.z);

    this.s.add(this.canvas);
  }



  update(dt) {
    // this._update_texture(dt);
  }
}


class ParticlePosition {
  constructor(cnt, rdrr) {
    this.setter = new ParticleSetter(rdrr);
    const option = {minFilter : THREE.NearestFilter, magFilter : THREE.NearestFilter };
    this.rdrr = rdrr;

    const width_ = Math.min(cnt, 16384);
    const height_ = Math.min(Math.floor(cnt / 16384) + 1, 16384);

    this.x = new THREE.WebGLRenderTarget(width_, height_, option);
    this.y = new THREE.WebGLRenderTarget(width_, height_, option);
    this.z = new THREE.WebGLRenderTarget(width_, height_, option);
    this.t = new THREE.WebGLRenderTarget(width_, height_, option);

    this.createpoint = { x : 0.0, y : 0.0};

    this.li = { type : "t",  value : this.x };
    this.ps = { type : "t",  value : this.y };
    this.vc = { type : "t",  value : this.z };
    this.dt = { type : "1f", value : 0.0 };
    this.ptr = { type : "1f", value : 0.0};

    this.s = new THREE.Scene();
    this.r = new THREE.Scene();
    this.c = new THREE.Camera();

    this.r.add(new THREE.Mesh(
      new THREE.PlaneGeometry(2.0, 2.0),
      new THREE.MeshBasicMaterial({ map : this.t })
    ));

    this.canvas = new THREE.Mesh(
      new THREE.PlaneGeometry(2.0, 2.0),
      new THREE.ShaderMaterial({
        uniforms : {
          unif_life : this.li,
          unif_position : this.ps,
          unif_velocity : this.vc,
          unif_deltatime : this.dt,
          unif_ptr : this.ptr,
        },
        fragmentShader : m_float + `
        uniform sampler2D unif_life;
        uniform sampler2D unif_position;
        uniform sampler2D unif_velocity;
        uniform float unif_deltatime;

        uniform float unif_ptr;

        ` + FloatToVec4_for_GLSL + `
        ` + Vec4ToFloat_for_GLSL + `

        varying vec2 vtex;
        void main(void) {
          float pos = Vec4ToFloat(texture2D(unif_position, vtex));
          float vel = Vec4ToFloat(texture2D(unif_velocity, vtex));
          float lif = Vec4ToFloat(texture2D(unif_life, vtex));

          pos = ((pos - M_FLOAT) * 2.0) * 255.0;
          vel = ((vel - M_FLOAT) * 2.0) * 255.0;
          lif = ((lif - M_FLOAT) * 2.0) * 255.0;

          pos += vel * 0.2 * unif_deltatime;

          if(lif < 0.0) pos = unif_ptr;

          pos = (pos / 255.0 * 0.5 + M_FLOAT);

          vec4 retcol = FloatToVec4(pos);
          vec4 retcold = texture2D(unif_position, vtex);
          gl_FragColor = vec4(retcol.rgb, 1.0);//retcol;//vec4(retcol.rgb, 1.0);
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

    this.setter.initTarget(this.x);
    this.setter.initTarget(this.y);
    this.setter.initTarget(this.z);

    this.s.add(this.canvas);

  }

  _update_texture(dt, pos, vel, lif, cpoint) {
    const {rdrr, r, s, c, t, ps, vc} = this;

    this.li.value = lif;
    this.ps.value = pos;
    this.vc.value = vel;
    this.dt.value = dt;
    if(cpoint == undefined) this.ptr.value = 0.0;
    else this.ptr.value = cpoint;
    rdrr.render(s, c, t);
    rdrr.render(r, c, pos);
  }

  update(dt, velocity, life) {
    const {rdrr, r, s, c, x, y, z, t, ps, vc} = this;
    var tmp;

    rdrr.autoClear = true;

    this._update_texture(dt, x, velocity.x, life.time, this.createpoint.x);
    this._update_texture(dt, y, velocity.y, life.time, this.createpoint.y);
    this._update_texture(dt, z, velocity.z, life.time);

    rdrr.autoClear = false;
  }

  setCreatePoint(x, y) {
    this.createpoint.x = x;
    this.createpoint.y = y;
  }
}
