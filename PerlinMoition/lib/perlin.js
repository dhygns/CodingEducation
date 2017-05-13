

class Perlin {
  constructor(width, height) {
    this.width = width << 0;
    this.height = height << 0 ;

    this.infoArray = [];
    this.infoArray.push(new Float32Array((this.width + 1) * (this.height + 1)));
    this.infoArray.push(new Float32Array((this.width + 1) * (this.height + 1)));

    for(var y = 0; y <= this.height; y++) {
      for(var x = 0; x <= this.width; x++) {
        const idx = x + y * this.width;

        var vec = new THREE.Vector2(Math.random() * 2.0 - 1.0, Math.random() * 2.0 - 1.0);
        vec = vec.normalize();
        this.infoArray[0][idx] = vec.x;
        this.infoArray[1][idx] = vec.y;
      }
    }

    this.tmp = new Float32Array(512 * 512);
    this.DataArray = new Uint8Array(512 * 512);
    this.Texture = new THREE.DataTexture(this.DataArray, 512, 512, THREE.AlphaFormat);
    this.Texture.minFilter = THREE.LinearFilter;
    this.Texture.magFilter = THREE.LinearFilter;


    for(var y = 0; y < 512; y++) {
      for(var x = 0; x < 512; x++) {
        // console.log(this.getIdx(x, y));
        const idx = x + y * 512;
        const value = (128 + this.proc((x + 0.5) / 512 * this.width, (y + 0.5) / 512 * this.height) * 128);
        this.DataArray[idx] = value;
        this.tmp[idx] = value;
        // if(y == 0) this.DataArray[idx] = 0;
        // if(value <= 0.0) console.log(value);
      }
    }
    // console.log(this.tmp);
  }

  getIdx(x, y) { return x + y * this.width; }

  gradient(y, x, t) { return this.infoArray[t << 0][this.getIdx(x, y)]; }
  lerp(a0, a1, w) { return (1.0 - w) * a0 + w * a1; }
  dotGridGradient(ix, iy, x, y) {
    var dx = x - ix;
    var dy = y - iy;
    return dx * this.gradient(iy, ix, 0) + dy * this.gradient(iy, ix, 1);
  }

  proc(x, y) {
    var x0 = x << 0;
    var x1 = x0 + 1;
    var y0 = y << 0;
    var y1 = y0 + 1;

    var sx = x - x0;
    var sy = y - y0;

    var n0, n1, ix0, ix1, value;
    n0 = this.dotGridGradient(x0, y0, x, y);
    n1 = this.dotGridGradient(x1, y0, x, y);
    ix0 = this.lerp(n0, n1, sx);
    n0 = this.dotGridGradient(x0, y1, x, y);
    n1 = this.dotGridGradient(x1, y1, x, y);
    ix1 = this.lerp(n0, n1, sx);
    value = this.lerp(ix0, ix1, sy);
    // console.log(value);
    return value;
  }

  update() {

    // for(var y = 0; y < 512; y++) {
    //   for(var x = 0; x < 512; x++) {
    //
    //     const idx = x + y * 512;
    //     // this.DataArray[idx] ++;
    //   }
    // }
    this.Texture.needsUpdate = true;
  }

}
