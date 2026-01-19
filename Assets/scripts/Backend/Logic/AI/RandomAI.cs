using System.Collections.Generic;
using UnityEngine;

// AI�����肵���u�s���v�̒��g������N���X
public class AIAction
{
    // �A�N�V�����̎�ށi�莝������u�� / �Տォ��ړ�����j
    public enum ActionType { PlaceFromHand, MoveOnBoard }

    public ActionType Type;
    public PylosCoordinate To;   // �ǂ��ɒu����
    public PylosCoordinate From; // �ǂ����玝���Ă��邩�i�ړ��̏ꍇ�̂ݎg�p�j
}

public class RandomAI : IAIAlgorithm
{
    private Rules rules;

    public RandomAI()
    {
        this.rules = new Rules();
    }

    // ----------------------------------------------------
    // 1. �ݒu�t�F�[�Y�̎v�l�i�莝���z�u or �Տ�ړ� �������_���I���j
    // ----------------------------------------------------
    public AIAction GetSetupMove(BoardModel board, PlayerModel me)
    {
        // �S�Ẳ\�ȍs�����X�g
        List<AIAction> validActions = new List<AIAction>();

        // === A. �莝������u���p�^�[����T�� ===
        if (me.BallCount > 0)
        {
            for (int z = 0; z < 4; z++)
            {
                int limit = 4 - z;
                for (int x = 0; x < limit; x++)
                {
                    for (int y = 0; y < limit; y++)
                    {
                        if (rules.CanPlaceAt(board, x, y, z))
                        {
                            validActions.Add(new AIAction
                            {
                                Type = AIAction.ActionType.PlaceFromHand,
                                To = new PylosCoordinate(x, y, z),
                                From = null
                            });
                        }
                    }
                }
            }
        }

        // === B. �Տ�̃{�[�����ړ�������p�^�[����T�� ===
        // �����F
        // 1. �����̃{�[���ŁA���������ԁiCanRemoveBall�j�ł��邱��
        // 2. �ړ��悪�󂢂Ă��āA�u�����ԁiCanPlaceAt�j�ł��邱��
        // 3. �s���X�̃��[���F�ړ���͌��̈ʒu��荂���i�łȂ���΂Ȃ�Ȃ��i�i�グ�j

        // �܂��u�������鎩���̃{�[���i�ړ����j�v��S���T��
        List<PylosCoordinate> movableBalls = new List<PylosCoordinate>();
        for (int z = 0; z < 3; z++) // ����(3�i��)����͏�ɍs���Ȃ��̂ŒT���Ȃ���OK
        {
            int limit = 4 - z;
            for (int x = 0; x < limit; x++)
            {
                for (int y = 0; y < limit; y++)
                {
                    if (board.HasBall(x, y, z) &&
                        board.GetColor(x, y, z) == me.Color &&
                        rules.CanRemoveBall(board, x, y, z))
                    {
                        movableBalls.Add(new PylosCoordinate(x, y, z));
                    }
                }
            }
        }

        // ���Ɂu�ړ���v��T���đg�ݍ��킹��
        foreach (var fromPos in movableBalls)
        {
            // �ړ���́A�ړ�������̒i (fromPos.Level + 1 �ȏ�)
            for (int z = fromPos.Level + 1; z < 4; z++)
            {
                int limit = 4 - z;
                for (int x = 0; x < limit; x++)
                {
                    for (int y = 0; y < limit; y++)
                    {
                        // �ړ��悪�u����ꏊ�Ȃ�A�A�N�V�������ɒǉ�
                        if (rules.CanPlaceAt(board, x, y, z))
                        {
                            validActions.Add(new AIAction
                            {
                                Type = AIAction.ActionType.MoveOnBoard,
                                To = new PylosCoordinate(x, y, z),
                                From = fromPos
                            });
                        }
                    }
                }
            }
        }

        // === ���� ===
        // �����ł��邱�Ƃ��Ȃ���� null
        if (validActions.Count == 0) return null;

        // �S���i�莝���z�u �{ �ړ��j�̒����烉���_����1�I��
        int randomIndex = Random.Range(0, validActions.Count);
        return validActions[randomIndex];
    }

    // ----------------------------------------------------
    // 2. ����t�F�[�Y�̎v�l�i������� or ���Ȃ� �������_���I���j
    // ----------------------------------------------------
    public PylosCoordinate GetRecoveryMove(BoardModel board, PlayerModel me)
    {
        // ����ł���{�[���̃��X�g
        List<PylosCoordinate> removableBalls = new List<PylosCoordinate>();

        for (int z = 0; z < 4; z++)
        {
            int limit = 4 - z;
            for (int x = 0; x < limit; x++)
            {
                for (int y = 0; y < limit; y++)
                {
                    if (board.HasBall(x, y, z) &&
                        board.GetColor(x, y, z) == me.Color &&
                        rules.CanRemoveBall(board, x, y, z))
                    {
                        removableBalls.Add(new PylosCoordinate(x, y, z));
                    }
                }
            }
        }

        // ���X�g�Ɂunull�v��ǉ����邱�ƂŁA�u������Ȃ��i�p�X�j�v��I�����Ɋ܂߂�
        // ���������A�{�[��������Ȃ����̓p�X�����ł��Ȃ��̂Œǉ��s�v�i������null���Ԃ�j
        if (removableBalls.Count > 0)
        {
            // �Ⴆ�΁u�I�����̐� + 1�v�͈̔͂Ń����_���ɂ���
            // count��3�Ȃ�A0,1,2,3 �̗������o���B3���o����u�p�X�v�Ƃ���
            int randomIndex = Random.Range(0, removableBalls.Count + 1);

            // �Ō�̃C���f�b�N�X����������u�p�X�i������Ȃ��j�v�Ƃ���
            if (randomIndex == removableBalls.Count)
            {
                return null; // ������Ȃ�
            }

            return removableBalls[randomIndex];
        }

        return null; // ����ł�����̂��Ȃ�
    }
}