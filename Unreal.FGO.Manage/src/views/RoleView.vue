<template>
  <div>
    <div>
      <div class="danmu-wrp">
        <div class="danmu-header"> <span class="main-title">{{id ? "编辑" : "添加"}}帐号</span> <span class="second-title">(请注意目前只支持B站帐号)</span>
          <a class="bili-btn" :class="{ok : validate}" @click="save">保存</a>
        </div>
        <div class="section-wrp">
          <div class="section clearfix first">
            <div class="left label">
              <div class="title"> <span class="main">用户名</span> </div>
            </div>
            <div class="left content">
              <div>
                <input type="text" v-model="username" class="bili-input" placeholder="输入bilibili用户名。例如mdzz">
              </div>
            </div>
          </div>
          <div class="section clearfix">
            <div class="left label">
              <div class="title"> <span class="main">密码</span> </div>
            </div>
            <div class="left content">
              <div>
                <input type="password" v-model="password" class="bili-input" placeholder="输入bilibili密码。">
              </div>
            </div>
          </div>
          <div class="section clearfix type last">
            <div class="left label">
              <div class="title"> <span class="main">平台类型</span> </div>
            </div>
            <div class="left content">
              <div class="input-group"> <label> <input type="radio" name="type" v-model="platform" class="bili-radio" value="1"> <span>IOS</span> </label>                </div>
              <div class="input-group last"> <label> <input type="radio" name="type" v-model="platform" class="bili-radio" value="3"> <span>Android</span> </label>                </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  </div>
</template>

<script>
  export default {
    name: 'role-view',
    components: {},
    data() {
      return {
        username: '',
        password: '',
        platform: 0,
        id: 0,
        loading: false
      }
    },
    methods: {
      save() {
        if (this.validate) {
          this.$store.dispatch('SET_ROLE', this.$data).then((res) => {
            if (res.data != null && res.data.code == 0)
              this.$router.push({ path: '/roles' })
          })
        }
      }
    },
    computed: {
      validate() {
        return this.username && this.password && this.platform;
      }
    },
    // on the server, only fetch the item itself
    //preFetch: fetchItem,
    // on the client, fetch everything
    beforeMount() {
      if (this.$route.params.id) {
        var role = this.$store.state.roles.find(r => r.id == this.$route.params.id);
        if (role) {
          this.id = role.id
          this.username = role.username
          this.password = role.password
          this.platform = role.platform_type
        }
      }
      // fetchItemAndComments(this.$store).then(() => {
      //   this.loading = false
      // })
    }
  }
</script>