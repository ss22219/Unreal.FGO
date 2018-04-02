<template>
  <div class="article-wrp" v-show="!loading">
    <empty v-if="!roles || !roles.length" msg="您还未添加一个帐号，因此无法管理帐号。" linkName="添加帐号" url="/role"></empty>
    <div v-if="roles.length">
      <div>
        <div class="article-header clearfix">
          <div class="right px-wrp clearfix">
            <div class="fq-wrp">
              <div class="dropdown">
                <a @click="add()"> <i class="iconfont icon-charge"></i><span> 添加帐号</span> </a>
              </div>
            </div>
          </div>
        </div>
        <div>
          <ul>
            <li class="article-item clearfix" v-for="role in roles">
              <div class="cover-wrp">
                <a><img v-if="role.face" class="cover-img" :src="role.face"></a>
              </div>
              <div class="meta-wrp">
                <div class="meta-header">
                  <div class="typename">{{role.platform_type == 1 ? 'iOS' : 'Android'}}</div>
                  <a class="title ellipsis">{{role.username}}</a>
                  <a class="title ellipsis">水晶：{{role.stone}}</a>
                </div>
                <div class="meta-body"></div>
                <div class="meta-footer">
                  <a class="edit" @click="edit(role)">
                    <i class="iconfont icon-edit"></i> 编辑 </a>
                  <a v-show="$store.state.battles[role.id]" class="edit" @click="battle(role)">
                    <i class="iconfont icon-edit"></i> 查看战斗 </a>
                  <a @click="task(role)" class="appeal item">查看计划</a>
                  <a @click="map(role)" class="appeal item">查看地图</a>
                  <a @click="items(role)" class="appeal item">查看物品</a>
                  <a @click="svts(role)" class="appeal item">查看从者</a>
                  <a class="delete" @click="del(role)"> <i class="iconfont icon-del2"></i> </a>
                </div>
              </div>
              <div class="del-wrp" style="height: 158px" v-show="deleteId === role.id">
<div class="del-title">是否确认删除此帐号？</div>
<div class="del-msg"></div>
<div class="btn-wrp">
  <a class="bili-btn ok" @click="realDel(role)">删除</a> <a class="bili-btn cancel" @click="clearDel(role)">取消</a>
</div>
</div>
</li>
</ul>
</div>
</div>
</div>
</div>
</template>

<script>
  import Empty from '../components/Empty.vue'
  export default {
    name: 'roles-view',
    components: { Empty },
    data() {
      return {
        delete: false,
        deleteId: 0
      }
    },
    methods: {
      svts(role){
        this.$router.push({ path: '/role/' + role.id + '/svts' })
      },
      battle(role){
          this.$router.push({ path: '/battle/' + role.id + '/0' })
      },
      items(role) {
        this.$router.push({ path: '/items/' + role.id })
      },
      task(role) {
        var task = this.$store.state.tasks.find(t => t.user_role_id == role.id)
        this.$router.push({ path: '/task/' + (task ? task.id : 'r' + role.id) })
      },
      map(role) {
        this.$router.push({ path: '/maps/' + role.id })
      },
      del(role) {
        this.deleteId = role.id
      },
      realDel(role) {
        this.$store.dispatch('DELETE_ROLE', role)
      },
      clearDel() {
        this.deleteId = -1
      }, edit(role) {
        this.$router.push({ path: '/role/' + role.id })
      }, add() {
        this.$router.push({ path: '/role' })
      }
    },
    computed: {
      loading() {
        return this.$parent.loading
      },
      roles() {
        return this.$store.state.roles
      }
    },
    // on the server, only fetch the item itself
    //preFetch: fetchItem,
    // on the client, fetch everything
    beforeMount() {
      // fetchItemAndComments(this.$store).then(() => {
      //   this.loading = false
      // })
    }
  }
</script>