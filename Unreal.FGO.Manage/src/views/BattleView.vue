<template>
    <div class="battle-warp" style="
    text-align: center;
    font-size: 92px;
    margin-top: 120px;
    text-shadow: 0 8px 9px #c3b8a6, 0px -2px 1px #00a1d6;
">
        <div v-show="!battleId" @click="start"><a>开始战斗</a></div>
        <div v-show="battleId" @click="end"><a>结束战斗</a></div>
    </div>
</template>

<script>
    export default {
        name: 'battle-view',
        components: {},
        data() {
            return {
                roleId: 0,
                questId: 0,
                battleId: false
            }
        },
        methods: {
            start() {
                this.$store.dispatch('BATTLE_SETUP', { roleId: this.roleId, questId: this.questId }).then(res =>{
                    console.log(res.data)
                    this.battleId = res.data.data.battleId
                })
            },
            end() {
                this.$store.dispatch('BATTLE_RESULT', { roleId: this.roleId, questId: this.questId, battleId: this.battleId }).then(res => {
                    this.$router.push({ path: '/maps/' + this.roleId })
                })
            }
        },
        computed: {
        },
        // on the server, only fetch the item itself
        //preFetch: fetchItem,
        // on the client, fetch everything
        beforeMount() {
            this.roleId = this.$route.params.roleId
            this.questId = this.$route.params.questId
            if (this.$store.state.battles[this.roleId])
                this.battleId = this.$store.state.battles[this.roleId].battleId
        }
        // fetchItemAndComments(this.$store).then(() => {
        //   this.loading = false
        // })
    }
</script>