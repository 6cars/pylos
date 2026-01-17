// AI�́u���ʃ��[���v�����߂�C���^�[�t�F�[�X
public interface IAIAlgorithm
{
    // 1. �ݒu�t�F�[�Y�F�ǂ����������߂�i�z�u or �ړ��j
    AIAction GetSetupMove(BoardModel board, PlayerModel me);

    // 2. ����t�F�[�Y�F�ǂ��������邩���߂�i��� or �p�X�j
    PylosCoordinate? GetRecoveryMove(BoardModel board, PlayerModel me);
}