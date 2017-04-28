
class Brush extends THREE.Object3D{
  constructor() {
    super();

    this.direction = { x : Math.sin(this.rotation.z), y : Math.cos(this.rotation.z) }
    this.velocity = { x : 0.0, y : 0.0 };

    this._velocity = { x : 0.0, y : 0.0};
    this._position = { x : 0.0, y : 0.0};
    this._scale = { x : 0.00, y : 0.00};

    this.scale.x = this.scale.y = 0.00;

    this._order = [];

    this._alpha = 1.0;
    this.alpha = { type : "1f", value : 1.0 };


    this.add(new THREE.Mesh(
      new THREE.PlaneGeometry(1.0, 1.0),
      new THREE.ShaderMaterial({
        uniforms : {
          unif_alpha : this.alpha,
        },
        transparent : true,
        vertexShader : `
        varying vec2 vary_tex;
        void main(void) {
          vary_tex = uv;
          gl_Position = projectionMatrix * modelViewMatrix * vec4(position.xy, 0.0, 1.0);
        }
        `,
        fragmentShader : `
        uniform float unif_alpha;
        varying vec2 vary_tex;

        float rand(vec2 co){
          return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
        }

        float brush(vec2 st) {
          return sin(st.y * 5.0 * 3.141592) + 1.0;
        }

        float circle(vec2 st) {
          return smoothstep( 0.5 + 0.5, 0.5, length(st));
        }

        float triangle(vec2 st) {
          return
            smoothstep( 0.50 + 0.30 , 0.50 - 0.30 , st.x + st.y * 0.5) *
            smoothstep( 0.50 + 0.30 , 0.50 - 0.30 , st.y - st.x * 2.0) *
            smoothstep( 0.25 + 0.15 , 0.25 - 0.15 , st.y);
        }

        void main(void) {
          vec2 st = vary_tex * 2.0 - 1.0;
          float alpha = triangle(st);
          gl_FragColor = vec4(0.0, 0.0, 0.0, alpha * 0.1 * unif_alpha);
        }
        `
      })
    ));
  }

  init(depth) {
    this.position.x = 0.0;
    this.position.y = 0.0;
    this.setnum( 1, -3.0, 0.0);
    this.setnum( 2, -1.0, 0.0);
    this.setnum( 3,  1.0, 0.0);
    this.setnum(-1,  1.0, 0.0);
    return this;
  }

  update(dt) {
    if(this._order.length > 0 && this._order[0]() == true) this._order.shift();
    // else {this._velocity.x = 0.0; this._velocity.y = 0.0;}
    this._update(dt);


  }

  _len(x, y) {
    return Math.sqrt(x * x + y * y);
  }

  _norm(x, y) {
    const len = this._len(x, y);
    return { x : x / len, y : y / len};
  }

  _update(dt) {

    this.velocity.x += (this._velocity.x - this.velocity.x) * dt * 4.0;
    this.velocity.y += (this._velocity.y - this.velocity.y) * dt * 4.0;

    this._position.x += this.velocity.x * dt * 4.0;
    this._position.y += this.velocity.y * dt * 4.0;

    this.position.x += (this._position.x - this.position.x) * dt * 4.0;
    this.position.y += (this._position.y - this.position.y) * dt * 4.0;

    this.scale.x += (this._scale.x - this.scale.x) * dt;
    this.scale.y += (this._scale.y - this.scale.y) * dt;

    this.alpha.value += (this._alpha - this.alpha.value) * 10.0 * dt;

    this.rotation.z = Math.PI * Math.random() * 2.0;////Math.atan(this.velocity.y / this.velocity.x);
  }

  _start(x, y) {
    this._alpha = 1.0;

    this._position.x = x;
    this._position.y = y;

    this._velocity.x = 0.0;
    this._velocity.y = 0.0;

    this.velocity.x = 0.0;
    this.velocity.y = 0.0;
    return true;
  }

  _finish() {
    this._alpha = 0.0;

    this._velocity.x = 0.0;
    this._velocity.y = 0.0;
    return true;
  }
  _moveTo(x, y) {
    const dir = this._norm(x - this._position.x, y - this._position.y);
    const len = this._len(x - this._position.x, y - this._position.y);
    this._velocity.x = dir.x * 0.5;
    this._velocity.y = dir.y * 0.5;

    const scl = Math.min(Math.max(0.5 * len, 0.5), 0.8);
    this._scale.x = this._scale.y = scl;

    if(len < 0.05) return true; else return false;
  }

  setnum(n, x, y) {
    switch(n) {
      case -1 :
      this._order.push(this. _start.bind(this, 0.0 + x,-2.0 + y));
      this._order.push(this._finish.bind(this));
      break;
      case 1 :
      this._order.push(this. _start.bind(this, 0.0 + x, 1.7 + y));
      this._order.push(this._moveTo.bind(this, 0.0 + x,-2.0 + y));
      this._order.push(this._finish.bind(this));
      break;
      case 2 :
      this._order.push(this. _start.bind(this,-1.0 + x, 0.5 + y));
      this._order.push(this._moveTo.bind(this, 0.0 + x, 1.3 + y));
      this._order.push(this._moveTo.bind(this, 0.6 + x, 0.5 + y));
      this._order.push(this._moveTo.bind(this,-1.0 + x,-1.3 + y));
      this._order.push(this._moveTo.bind(this, 1.0 + x,-1.3 + y));
      this._order.push(this._finish.bind(this));
      break;
      case 3 :
      this._order.push(this. _start.bind(this,-1.0 + x, 0.5 + y));
      this._order.push(this._moveTo.bind(this, 0.0 + x, 1.3 + y));
      this._order.push(this._moveTo.bind(this, 0.6 + x, 0.5 + y));
      this._order.push(this._moveTo.bind(this,-0.3 + x, 0.0 + y));
      this._order.push(this._moveTo.bind(this, 0.6 + x,-0.5 + y));
      this._order.push(this._moveTo.bind(this, 0.0 + x,-1.5 + y));
      this._order.push(this._moveTo.bind(this,-1.0 + x,-0.5 + y));
      this._order.push(this._finish.bind(this));
      break;
    }
  }
}
